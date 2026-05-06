#include <iostream>
#include <vector>
#include <string>
using namespace std;

int main() {
    int n, bits;

    cout << "Enter number of stations: ";
    cin >> n;

    cout << "Enter number of bits in each address: ";
    cin >> bits;

    vector<string> stations(n);

    cout << "\nEnter binary addresses:\n";
    for (int i = 0; i < n; i++) {
        cout << "Station " << i << ": ";
        cin >> stations[i];
    }

    vector<bool> active(n, true);

    for (int bit = 0; bit < bits; bit++) {

        cout << "\nBit Position " << bit << ":\n";

        int resultBit = 0;

        // OR operation
        for (int i = 0; i < n; i++) {
            if (active[i]) {
                int b = stations[i][bit] - '0';
                resultBit = resultBit | b;
            }
        }

        cout << "Result after OR = " << resultBit << endl;

        // Elimination
        for (int i = 0; i < n; i++) {
            if (active[i]) {
                int b = stations[i][bit] - '0';

                if (b == 0 && resultBit == 1) {
                    active[i] = false;
                    cout << "Station " << i << " (" << stations[i] << ") : eliminated\n";
                } else {
                    cout << "Station " << i << " (" << stations[i] << ") : survives\n";
                }
            }
        }
    }

    for (int i = 0; i < n; i++) {
        if (active[i]) {
            cout << "\nWinner: Station " << i << " (" << stations[i] << ")\n";
        }
    }

    return 0;
}