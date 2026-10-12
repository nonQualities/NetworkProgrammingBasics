defmodule TcpLab.Server do
  @port 8080
  @eom "<|EOM|>"

  def start_link(_opts) do
    Task.start_link(fn -> listen() end)
  end

  defp listen do
    options = [
      :binary,
      packet: :raw,
      active: false,
      reuseaddr: true,
      ip: {127, 0, 0, 1}
    ]

    case :gen_tcp.listen(@port, options) do
      {:ok, listen_socket} ->
        IO.puts("Listening on 127.0.0.1:#{@port}")
        accept_loop(listen_socket)

      {:error, reason} ->
        raise "Could not start TCP server: #{inspect(reason)}"
    end
  end

  defp accept_loop(listen_socket) do
    case :gen_tcp.accept(listen_socket) do
      {:ok, client_socket} ->
        IO.puts("Client connected")

        handle_client(client_socket)
        accept_loop(listen_socket)

      {:error, reason} ->
        IO.puts("Accept failed: #{inspect(reason)}")
    end
  end

  defp handle_client(socket) do
    try do
      case receive_message(socket, "") do
        {:ok, message} ->
          IO.puts("Received: #{message}")

          :gen_tcp.send(socket, "<|ACK|>")
          |> case do
            :ok -> :ok
            {:error, reason} ->
              IO.puts("Send failed: #{inspect(reason)}")
          end

        {:error, reason} ->
          IO.puts("Receive failed: #{inspect(reason)}")
      end
    after
      :gen_tcp.close(socket)
      IO.puts("Connection closed")
    end
  end

  defp receive_message(socket, accumulated) do
    case :gen_tcp.recv(socket, 0) do
      {:ok, data} ->
        message = accumulated <> data

        case :binary.match(message, @eom) do
          {index, _length} ->
            {:ok, binary_part(message, 0, index)}

          :nomatch ->
            receive_message(socket, message)
        end

      {:error, reason} ->
        {:error, reason}
    end
  end
end