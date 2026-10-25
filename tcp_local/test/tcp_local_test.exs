defmodule TcpLocalTest do
  use ExUnit.Case
  doctest TcpLocal

  test "greets the world" do
    assert TcpLocal.hello() == :world
  end
end
