using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace OpenVpnManagement {
  public class Manager : IDisposable {

    public enum Signal {
      Hup,
      Term,
      Usr1,
      Usr2
    }

    public enum State {
      Connecting,
      Wait,
      Auth,
      GetConfig,
      AssignIP,
      AddRoutes,
      Connected,
      Reconnecting,
      Exiting
    }

    private Socket socket;
    private const int bufferSize = 1024;
    private string? ovpnFilePath;

    private void RunOpenVpnProcess() {
      Process prc = new Process();
      prc.StartInfo.CreateNoWindow = false;
      prc.EnableRaisingEvents = true;
      prc.StartInfo.Arguments = string.Format("--config {}", ovpnFilePath);
      prc.StartInfo.FileName = "openvpn.exe";
      prc.StartInfo.WorkingDirectory = @"C:\Program Files\OpenVPN\config";
      prc.Start();
    }
    public Manager(string host, int port, string? ovpnFilePath) {
      if (ovpnFilePath != null && ovpnFilePath.ToString() != string.Empty) {
        var ovpnFilePath2 = ovpnFilePath.ToString();
        var res = File.ReadAllLines(ovpnFilePath2).Where(x => x == "management ");
        if (!res.Any()) {
          File.AppendAllText(ovpnFilePath2, string.Format("management {} {}", host, port));    
        }

        RunOpenVpnProcess();
      }

      socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(host, port);
      GreetServer();
    }

    public string GetStatus() {
      return this.SendCommand("status");
    }

    /// <summary>
    /// State
    /// </summary>
    /// <returns></returns>
    public State GetState() {
      var res = this.SendCommand("state");
      return State.Auth; //todo
    }

    public string GetState(int n = 1) {
      return this.SendCommand(string.Format("state {}", n.ToString()));
    }

    public string GetStateAll() {
      return this.SendCommand("state all");
    }

    public string SetStateOn() {
      return this.SendCommand("state on");
    }

    public string SetStateOnAll() {
      return this.SendCommand("state on all");
    }

    public string GetStateOff() {
      return this.SendCommand("state off");
    }

    public string GetVersion() {
      return this.SendCommand("version");
    }

    public string GetPid() {
      return this.SendCommand("pid");
    }

    public string SendSignal(Signal sgn) {
      return this.SendCommand(string.Format("SIG{}", sgn.ToString().ToUpper()));
    }

    public string Mute() {
      return this.SendCommand("pid");
    }

    public string GetEcho() {
      return this.SendCommand("echo");
    }

    public string GetHelp() {
      return this.SendCommand("help");
    }

    public string Kill(string name) {
      return this.SendCommand(string.Format("kill {}", name));
    }

    public string Kill(string host, int port) {
      return this.SendCommand(string.Format("kill {}:{}", host, port));
    }

    public string GetNet() {
      return this.SendCommand("net");
    }

    /// <summary>
    /// Logs
    /// </summary>
    /// <returns></returns>
    public string GetLogAll() {
      return this.SendCommand("state off");
    }

    public string SetLogOn() {
      return this.SendCommand("log on");
    }

    public string SetLogOnAll() {
      return this.SendCommand("log on all");
    }

    public string SetLogOff() {
      return this.SendCommand("log off");
    }

    public string GetLog(int n = 1) {
      return this.SendCommand(string.Format("log {}", n));
    }

    public string SendMalCommand() {
      return this.SendCommand("fdsfds");
    }

    private static string TreamRetrievedString(string s) {
      return s.Replace("\0", "");
    }

    private void GreetServer() {
      var bf = new byte[bufferSize];
      int rb = socket.Receive(bf, 0, bf.Length, SocketFlags.None);
      if (rb < 1) {
        throw new SocketException();
      }
    }

    private string SendCommand(String cmd) {
      socket.Send(Encoding.Default.GetBytes(cmd + "\r\n"));

      var bf = new byte[bufferSize];
      var sb = new System.Text.StringBuilder();
      int rb;
      string s = "";
      while (true) {
        rb = socket.Receive(bf, 0, bf.Length, 0);
        s = Encoding.UTF8.GetString(bf).Replace("\0", "");
        if (rb < bf.Length) {
          if (s.Contains("\r\nEND")) {
            var a = s.Substring(0, s.IndexOf("\r\nEND"));
            sb.Append(a);
          } else if (s.Contains("SUCCESS: ")) {
            var a = s.Replace("SUCCESS: ", "").Replace("\r\n", "");
            sb.Append(a);
          } else if (s.Contains("ERROR: ")) {
            var msg = s.Replace("ERROR: ", "").Replace("\r\n", "");
            throw new ArgumentException(msg);
          }

          break;
        } else {
          sb.Append(s);
        }
      }

      return sb.ToString();
    }

    public void Dispose() {
      if (socket != null) {
        if (ovpnFilePath != null && ovpnFilePath != string.Empty) {
          SendSignal(Signal.Term);
        }

        socket.Dispose();
      }
    }
  }
}
