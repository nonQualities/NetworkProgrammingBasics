
I assume you know CSMA, but if you do not here is a quick brief: CSMA or Carrier Sense Multiple Access is a MAC protocol that manages and solves multiple access problems, by sensing the channel before transmitting. It has a few types but the ones we are interested here are the 1 persistent and the p persistent: The 1 persistent is as follows: senses if the channel is idle, if yes transmits, if no continuously keeps sensing until its idle. The p persistent is as follows: senses, if idle transmits, if not keeps sensing if idle, and then sends with a probability $p$ if doesn't transmit waits for random time and repeats. ( oversimplification might have killed some preciseness, but we duhhh. Read the Wiki page...I am really tired after writing this whole thing!!! )

---

So this started, like all the worst assignments do, with a dangerous sense of familiarity — "yeah yeah, CSMA, collisions, backoff, I've seen this" — which is precisely the kind of premature confidence that precedes genuine humiliation, because somewhere between writing the `Node` class and watching the terminal shuffle between `IDLE`, `TRANSMITTING`, and `COLLISION` like a socially anxious traffic light (I mean, watch the video attached, it IS a traffic light for real), it became clear that this thing is beyond simulating packets. and really a  trial on simulating _behavior under constraint_, and that Kleinrock and Tobagi had already worked out all the math for exactly this behaviour in 1975, which is simultaneously humbling and vindicating.

The structure of the program is simple. We define a `Node`, an agent that at any moment exists in one of three states — `Sensing`, `Transmitting`, or `BackingOff`:

```csharp
public enum NodeState
{
    Sensing,
    Transmitting,
    BackingOff
}
```

If you stare at that long enough, it's a complete psychological model: every node is either _watching_, _acting_, or _regretting_ — and the paper formalizes this almost word for word when it defines a _ready terminal_ in Section II as one that "*has a packet ready for transmission at this instant (either a new packet just generated or a previously conflicted packet rescheduled for transmission at this instant)*." `Sensing` is the ready state. Everything else is consequence.

The `SharedMedium` is the well, the medium, they all inhabit, resolving each tick's collective fate with simplicity: zero transmitters means `Idle`, one means `Busy` and increments `SuccessfulPackets`, more than one means `Collision` and increments `TotalCollisions`. This corresponds directly to what the paper calls the **noncapture assumption**, stated in Section II: _"the overlap of any fraction of two packets results in destructive interference and both packets must be retransmitted."_ The `SharedMedium.ResolveState()` method is that assumption, operationalized. 

---

## Why 1-Persistent Fails, or: The Synchronized Certainty Catastrophe  or : I wanted to give it a cool name xx

The most instructive thing to do first is look at `OnePersistentTick`, because understanding why it fails is the entire motivation for everything else in the program:

```csharp
case (NodeState.Sensing, ChannelState.Idle):
    State = NodeState.Transmitting;
    break;
```

Idle channel detected meaning transmit immediately, with probability one. Pure confidence. 
And Kleinrock and Tobagi, on page 1402 (its page number on the journal), describe this protocol with equally decisive language: _"If the channel is sensed idle, it transmits the packet with probability one."_ They even name it 1-persistent precisely because the persistence parameter is 1 — complete commitment to transmitting the moment the channel is free.

The paper then immediately identifies why this is a problem, and it's worth quoting the diagnosis directly because it's so clean: _"whenever two or more terminals become ready during a transmission period, they wait for the channel to become idle and then they all transmit with probability one. A conflict will also occur with probability one!"_ That exclamation mark is doing a lot of work lol. Two or more nodes, all watching the same channel, all waiting for the same idle signal, all executing the same deterministic rule they synchronize perfectly, and perfect synchronization in a contention environment produces guaranteed mutual destruction (sounds like toxic love 🤧)

The paper's Figure 5(I will link the paper up) shows the throughput curves for 1-persistent CSMA under various values of $a$ (the normalized propagation delay $\tau/T$), and they all exhibit the same pathology: throughput peaks at some moderate load $G$ and then collapses, because as more nodes contend, the synchronized collision behavior dominates and $S$ plummets toward zero. The simulation replicates this empirically — run `OnePersistentTick` with enough nodes and watch `TotalCollisions` climb relentlessly while `SuccessfulPackets` stagnates.

---

## The p-Persistent Protocol, or: Restraint as Engineering or: I couldn't find a cool name

So you introduce p-persistence, and this is where the assignment stops being a programming exercise and becomes a confrontation with why randomness is not chaos but rather _the most sophisticated form of coordination available to agents who cannot talk to each other_.(I am really proud of this line...mwahahaha)

The paper introduces the p-persistent protocol in Section II with the following motivation — (i like em dashes, not all that em-dashes is AI) and it's worth reading carefully: _"The idea of randomizing the starting time of transmission of packets accumulating at the end of a TP suggests itself for interference reduction and throughput improvement. The scheme consists of including an additional parameter p, the probability that a ready packet persists (1-p being the probability of delaying transmission by τ seconds)."_


```csharp
case (NodeState.Sensing, ChannelState.Idle):
    double chance = _rng.NextDouble();
    if(chance <= _pTransmit)
        State = NodeState.Transmitting;
    else
    {
        BackOffTimer = 1;
        State = NodeState.BackingOff;
    }
    break;
```

This line — `double chance = _rng.NextDouble()` technically, in the sense that it's performing a stochastic binary decision; but mathematically, what it's actually doing is sampling a **Bernoulli random variable** $X \sim \text{Bernoulli}(p)$, and philosophically, it is the idea that a rational agent in a contention environment should voluntarily abstain from acting even when acting is _available_, because the act of abstention, when performed probabilistically and independently across all agents, is precisely what creates the statistical room for coordination to emerge. ( uhm uhm... How am I writing these lines.. (just kidding 5 rewrites(brackets are like footnotes to footnotes(or just lisp))))

Now, the paper formalizes exactly why this works. With $n$ nodes simultaneously sensing an idle channel, the probability that exactly one of them transmits i.e., the probability that something useful happens is:

$$P_{\text{success}}(n) = n p (1-p)^{n-1}$$

This is the PMF of a binomial distribution evaluated at $k = 1$, and it is not just a formula but a _condition for viability_: the system can only produce successful transmissions when this expression is non-negligible, and the only way to make it non-negligible under high contention ($n$ large) is to choose $p$ small enough. 1-persistent CSMA sets $p = 1$, which gives $P_{\text{success}} = 0$ for any $n \geq 2$ ,exactly the guaranteed collision the paper warned about earlier

The paper's Figure 7 (again, link to the paper) demonstrates this empirically for the throughput $S(G, p, a)$ — a family of curves parametrized by $p$, showing that lower values of $p$ produce higher peak throughput at the cost of slightly higher idle time, while larger $p$ produces collision-dominated collapse at moderate loads. There is an optimal $p$ for each value of $a$, and the paper notes (Figure 8) that for $a = 0.01$, this optimal is around $p = 0.03$, which is dramatically lower than 1. Our code uses $p = 0.3$ as the default:

```csharp
private readonly double _pTransmit = 0.3;
```

which is already much more conservative than 1-persistent but still higher than the analytically optimal value 

## The Geometric Distribution Is Living Inside the BackOffTimer

When a node defers with probability $(1-p)$, it sets `BackOffTimer = 1` and enters `BackingOff`. One tick passes, the timer decrements to zero, and the node returns to `Sensing` — where it performs the same Bernoulli trial again. If it defers again, another tick of waiting. This process repeats until the node finally decides to transmit.

The paper describes this mechanism in Section II as the geometric retry structure: the node transmits "with probability $p$, or with probability $1-p$, delays the transmission of the packet by $\tau$ seconds (i.e., one slot). If at this new point in time, the channel is still detected idle, the same process is repeated." This is a sequence of independent Bernoulli trials, and the number of slots elapsed before the first success follows a **geometric distribution**: (sidebar again: I am not that good with statistics and probability, I cannot prove the equations here ground up, they are directly picked from the paper, with thier interpretation of them. )

$$P(T = k) = (1-p)^{k-1} \cdot p, \qquad \mathbb{E}[T] = \frac{1}{p}$$

With $p = 0.3$, a node expects to wait about 3.3 mini-slots before transmitting. With $p = 0.03$ (the paper's optimal for $a = 0.01$), it expects to wait about 33 slots — much more conservative, much less likely to collide with its neighbors.

## Exponential Backoff, or: The System Has Trauma Memory or: This name was suggested by Gemini.

The exponential backoff section is where the program starts to feel like it has _learned from experience_ in a way that actual people (like me!) often don't manage:

```csharp
private void ExponentialBackOff()
{
    State = NodeState.BackingOff;
    CollisionCount++;

    int maxK = Math.Min(CollisionCount, 10);
    int maxContentionWindow = 1 << maxK;
    
    int R = _rng.Next(0, maxContentionWindow);
    BackOffTimer = R > 0 ? R : 1;
}
```

After each collision, `CollisionCount` increments, and the contention window grows as $2^k$, capped at $k = 10$. The random backoff $R$ is drawn from:

$$R \sim \mathcal{U}{0, 2^k - 1}, \qquad \mathbb{E}[R] = \frac{2^k - 1}{2}$$

This connects directly to what the paper calls the _randomly distributed retransmission delay_, introduced in Section II as the mechanism for avoiding "continuously repeated conflicts" — _"some scheme must be devised for introducing a random retransmission delay, spreading the conflicting packets over time."_ The paper's Assumption 1 in Section III then formalizes this: the average retransmission delay $\bar{X}$ must be _"large compared to T"_ (the packet transmission time), and Assumption 2 requires that the resulting retransmission process be approximately independent.

The detail `BackOffTimer = R > 0 ? R : 1` is  charming, refusing to allow zero delay under any circumstances — ensuring that a node which just caused a collision cannot immediately re-enter contention in the next tick. It's a one-line enforcement of the paper's whole retransmission philosophy: you collided, you wait..

The cap at $k = 10$ (maximum window of 1024 slots) also matters, and the paper provides indirect justification, but didn't understand that that nicely, and at this point in time I was tired really.


## The Throughput Formula, or: The Math , the Code Is Living Without Knowing

Zoom out to what the paper actually cares about the aggregate channel performance and you arrive at the two central quantities: $G$ (offered load, in packets per transmission time $T$) and $S$ (throughput, the normalized rate of successful transmissions), related by the renewal-theoretic identity in equation (4):

$$S = \frac{\bar{U}}{\bar{B} + \bar{I}}$$

where $\bar{U}$ is the expected time during a cycle that the channel carries a successful transmission, $\bar{B}$ is the expected busy period, and $\bar{I}$ is the expected idle period. The paper derives this for p-persistent CSMA in Section IV-D, arriving at the full throughput expression in equation (46): (again, the derivation is not my strong suit, yeah, it is what it is. I am really taking the math for granted.)

$$S(G,p,a) = \frac{P_s' + \frac{1-\pi_0}{\pi_0} P_s}{a\bar{t}' + a\bar{t}\frac{1-\pi_0}{\pi_0} + \frac{1+a}{\pi_0} + \frac{a}{1-e^{-g}}}$$

where $\pi_0 = \exp{-(1+a)G}$ (the probability that zero packets accumulate at the end of a transmission period), $P_s$ is the success probability over an interior transmission period, $P_s'$ is the success probability over the first transmission period of a busy period, $\bar{t}$ is the average IRTD within a busy period, and $\bar{t}'$ is the average IRTD at the start of a busy period. Every one of these quantities is itself a function of $G$, $p$, and $a$, derived under the Poisson arrival assumption and the independence approximation.

The code doesn't compute any of this. The code is performing a experiment whose long-run average approximates equation (46). The paper derived (46) analytically from first principles. The two approaches should agree, and the paper shows that they do but let's not get into those math.

---

## The Problem with the RNG, and Why It Matters

There is one implementation detail that deserves more credit than it usually gets: the random number generator.

```csharp
_rng = new MersenneTwister(RandomSeed.Robust());
```

Each `Node` gets its own independently seeded `MersenneTwister`. This sounds like overkill until you read Assumption 2 in the paper's Section III, which requires that _"the interarrival times of the point process defined by the start times of all the packets plus retransmissions are independent and exponentially distributed."_ The paper immediately acknowledges that this assumption is violated by the protocols themselves — it's introduced for analytic tractability — but then justifies it empirically in Section V by showing that simulation results match the analytic model when the retransmission delay is sufficiently large.

The validity of that empirical match depends entirely on the retransmission decisions of different nodes being statistically independent. If the nodes shared an RNG, or used poorly seeded generators with correlated streams, their decisions would be subtly correlated, and the independence assumption would be violated not just analytically but empirically — the simulation would be lying to you about the protocol's behavior. Separate, robustly seeded Mersenne Twisters per node uphold the independence assumption in the only way a discrete simulation can: by ensuring that each node's Bernoulli trials genuinely don't inform each other's.

---

## What the Simulation Actually Shows

When you run the simulation long enough, watch `SuccessfulPackets` and `TotalCollisions` accumulate, and compute the ratio, you're sampling from the distribution the paper characterizes in Figure 7 — a family of S-shaped throughput curves parametrized by $p$, each peaking at an optimal $G$ and then declining as contention drives $S$ toward zero.

The paper's Table I is the most useful sanity check: for $a = 0.01$ (which is a reasonable approximation of our simulation's $a \approx 0$ regime), the capacity of 0.1-persistent CSMA is 0.791, while 1-persistent achieves only 0.529. The performance gap between `OnePersistentTick` and `PPersistentTick` that you observe in the terminal is not a qualitative intuition — it's a quantitative gap that Kleinrock and Tobagi computed on an IBM 360/91 in 1975, and it's been sitting there, waiting for someone to rediscover it with a C# console app and a 300ms sleep delay.

---

## The Slightly Humbling Conclusion

Here is what hits you, eventually, after enough time watching nodes flip between `SENSING` and `BACKING OFF 7` on a terminal that has no business being this dramatic: the whole system — the throughput, the stability, the fact that it doesn't immediately descend into infinite collision loops — works _because_ of randomness, not despite it.

The paper makes this point structurally, by showing that 1-persistence (pure determinism upon channel idleness) leads to guaranteed conflicts, while p-persistence (probabilistic restraint) enables the throughput expressions of Section IV to converge to non-trivial values. But there's something more visceral about watching it happen in real time on a terminal — each node independently drawing a number between 0 and 1, comparing it to 0.3, and deciding, on that basis alone, whether to act or wait; and from that multiplication of tiny independent hesitations, something that looks remarkably like coordination emerging without any node ever talking to any other.

That's the principle the paper is really about, underneath all the Laplace transforms and generating functions and renewal-theoretic arguments: when agents share a resource without central authority, the only scalable path to avoiding catastrophe is to let chance mediate access.

Which means the most intellectually significant line in the whole program isn't the state machine, isn't the backoff calculation, isn't even the `SharedMedium` arbitrating collisions — it's this:

```csharp
double chance = _rng.NextDouble();
if(chance <= _pTransmit)
    State = NodeState.Transmitting;
```

One comparison. One Bernoulli trial. And from that, multiplied across $n$ independent nodes across discrete time, you get $np(1-p)^{n-1}$ probability of success per slot, a geometric distribution of waiting times, a system whose throughput converges toward equation (46), and an empirical realization of a result that required Kleinrock and Tobagi 16 pages of IEEE Transactions prose to derive properly.

And that, for something that started as a “simple networking assignment,” is doing an unreasonable amount of intellectual work
