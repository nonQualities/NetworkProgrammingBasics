defmodule TcpLab.Client do
  @server_ip {127, 0, 0, 1}
  @port 8080
  @eom "<|EOM|>"

  def run do
    case :gen_tcp.connect(
           @server_ip,
           @port,
           [:binary, active: false, packet: :raw],
           5_000
         ) do
      {:ok, socket} ->
        try do
          IO.write("Enter a message: ")

          message =
            IO.gets("")
            |> String.trim_trailing()

          payload = message <> @eom

          case :gen_tcp.send(socket, payload) do
            :ok ->
              IO.puts("Sent: #{message}")

              case :gen_tcp.recv(socket, 0, 5_000) do
                {:ok, "<|ACK|>"} ->
                  IO.puts("Received acknowledgement: <|ACK|>")

                {:ok, response} ->
                  IO.puts("Unexpected response: #{inspect(response)}")

                {:error, reason} ->
                  IO.puts("Receive failed: #{inspect(reason)}")
              end

            {:error, reason} ->
              IO.puts("Send failed: #{inspect(reason)}")
          end
        after
          :gen_tcp.close(socket)
        end

      {:error, reason} ->
        IO.puts("Connection failed: #{inspect(reason)}")
    end
  end
end