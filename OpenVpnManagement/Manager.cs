using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace OpenVpnManagement {
    public class Manager {
        enum Signal {
            Hup,
            Term,
            Usr1,
            Usr2
        }
        
        private String ovpnFileName; //todo
        private String host;
        private int port;
        private int timeout;
        private Socket socket; 

        public void Manager(String host = "localhost", int port = 1194, int timeout = 10) {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                socket.Connect(host, port);
            } catch (Exception ex) {
                //todo
            }
        }

        public string GetStatus() {
            return this.SendCommand("status");
        }

        /// <summary>
        /// State
        /// </summary>
        /// <returns></returns>
        public string GetState() {
            return this.SendCommand("state");
        }

        public string GetState(int n = 1) {
            return this.SendCommand(string.Format("state {}", n.ToString());
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

        public string GetGetPid() {
            return this.SendCommand("pid");
        }

        public string SendSignal(Signal sgn) {
            return this.SendCommand(string.Format("SIG{}", sgn.ToString().ToUpper());        
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

        public string Net() {
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


        private string SendCommand(String cmd) {
            byte[] buffer = Encoding.Default.GetBytes(cmd);
            socket.Send(buffer, 0, buffer.Length, 0);
            buffer = new byte[255];
            int rec = socket.Receive(buffer, 0, buffer.Length, 0);

            return rec.ToString();
        }
    }
}
