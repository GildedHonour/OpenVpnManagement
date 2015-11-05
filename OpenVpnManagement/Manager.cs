using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVpnManagement {
    public class Manager {
        enum Signal {
            Hup,
            Term,
            Usr1,
            Usr2
        }

        private String ovpnFileName;
        private String host;
        private int port;
        private int timeout;
        public void Manager(String ovpnFileName) {
            this.ovpnFileName = ovpnFileName;
        }

        public void Manager(String host, ) {
            this.ovpnFileName = ovpnFileName;
        }

        public String GetStatus() {
            var a = this.SendCommand("status");
        }

        public String GetStats() {
            var a = this.SendCommand("load-stats");
        }

        public String GetVersion() {
            return this.SendCommand("version");
        }

        public String GetPid() {
            return this.SendCommand("pid");
        }

        public String SendSignal(Signal sgn) {
            return this.SendCommand(string.Format("SIG{}", sgn.ToString().ToUpper());        
        }

        private String SendCommand(String cmd) {

        }
    }

}
