using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.IO;

namespace OpenVpnManagement {
  public class Manager : IDisposable {

    public enum Signal {
      Hup,
      Term,
      Usr1,
      Usr2
    }

    Socket socket;
    const int bufferSize = 1024;
    string ovpnFileName;
    const string eventName = "MyOpenVpnEvent";
    Process prc;
    readonly string openVpnExePath;
    bool isPassFileAdded = false;

    private void RunOpenVpnProcess() {
      prc = new Process();
      prc.StartInfo.CreateNoWindow = false;
      prc.EnableRaisingEvents = true;
      prc.StartInfo.Arguments = string.Format("--config {0}  --service {1} 0", ovpnFileName, eventName);
      prc.StartInfo.FileName = openVpnExePath;
      prc.Start();
    }

    public Manager(string host, int port, string ovpnFileName, string? userName = null, string? password = null, string openVpnExeFileName = @"C:\Program Files\OpenVPN\bin\openvpn.exe") {
      this.openVpnExePath = openVpnExeFileName;
      if (!string.IsNullOrEmpty(ovpnFileName)) {
        if (!Path.IsPathRooted(ovpnFileName)) {
          this.ovpnFileName = Path.Combine(Directory.GetCurrentDirectory(), ovpnFileName);
        }

        var ovpnFileLines = File.ReadAllLines(ovpnFileName);

        //management
        if (!ovpnFileLines.Where(x => x.StartsWith("management")).Any()) {
          File.AppendAllText(ovpnFileName, string.Format("{0}management {1} {2}", Environment.NewLine, host, port.ToString()));
        }

        //auto login
        var maybeAuthUserPass = ovpnFileLines.Where(x => x.StartsWith("auth-user-pass"));
        var passFileName = Path.Combine(Path.GetTempPath(), "ovpnpass.txt");
        if (maybeAuthUserPass.Any()) {
          if (userName == null || password == null) {
            throw new ArgumentException("Username or password cannot be null");
          }

          // create a credentials file
          File.WriteAllLines(passFileName, new string[] { userName.Value, password.Value });

          // add its path the ovpn file and write it back to the ovpn file
          var idx = Array.FindIndex(ovpnFileLines, x => x.StartsWith("auth-user-pass"));
          ovpnFileLines[idx] = string.Format("auth-user-pass {0}", passFileName);
          File.WriteAllLines(ovpnFileName, ovpnFileLines);
        } else {
          if (userName != null || password != null) {
            throw new ArgumentException("Username or password are provided but the *.ovpn file doesn't have the line 'auth-user-pass'");
          }
        }

        RunOpenVpnProcess();
      }

      socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(host, port);
      SendGreeting();
    }

    public string GetStatus() {
      return this.SendCommand("status");
    }

    public string GetState() {
      return this.SendCommand("state");
    }

    public string GetState(int n = 1) {
      return this.SendCommand(string.Format("state {0}", n));
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
      return this.SendCommand(string.Format("signal SIG{0}", sgn.ToString().ToUpper()));
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
      return this.SendCommand(string.Format("kill {0}", name));
    }

    public string Kill(string host, int port) {
      return this.SendCommand(string.Format("kill {0}:{1}", host, port));
    }

    public string GetNet() {
      return this.SendCommand("net");
    }

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
      return this.SendCommand(string.Format("log {0}", n));
    }

    public string SendMalCommand() {
      return this.SendCommand("fdsfds");
    }

    private static string TreamRetrievedString(string s) {
      return s.Replace("\0", "");
    }

    private void SendGreeting() {
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
      string str = "";
      while (true) {
        Thread.Sleep(100);
        rb = socket.Receive(bf, 0, bf.Length, 0);
        str = Encoding.UTF8.GetString(bf).Replace("\0", "");
        if (rb < bf.Length) {
          if (str.Contains("\r\nEND")) {
            var a = str.Substring(0, str.IndexOf("\r\nEND"));
            sb.Append(a);
          } else if (str.Contains("SUCCESS: ")) {
            var a = str.Replace("SUCCESS: ", "").Replace("\r\n", "");
            sb.Append(a);
          } else if (str.Contains("ERROR: ")) {
            var msg = str.Replace("ERROR: ", "").Replace("\r\n", "");
            throw new ArgumentException(msg);
          } else {

            //todo
            continue;
          }

          break;
        } else {
          sb.Append(str);
        }
      }

      return sb.ToString();
    }

    public void Dispose() {
      if (socket != null) {
        if (ovpnFileName != null) {
          SendSignal(Signal.Term);
        }
      }

      socket.Dispose();
      EventWaitHandle resetEvent = EventWaitHandle.OpenExisting(eventName);
      resetEvent.Set();
      prc.Close();
    }
  }
}
