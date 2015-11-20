OpenVpnManagement
====

C# OpenVPN interface

### Example
```
// Connect to the running openvpn process
using (var mng = new Manager("127.0.0.1", 8888)) {
  var res1 = mng.GetPid();
  Console.WriteLine(res1);

  var res2 = mng.GetHelp();
  Console.WriteLine(res2);

  var res3 = mng.SendMalCommand();
  // Exception is thrown
}

// Run openvpn and connect to it
using (var mng = new Manager("127.0.0.1", 8888, "Andorra.AndorralaVella.TCP.ovpn")) {
  var res1 = mng.GetPid();
  Console.WriteLine(res1);

  var res2 = mng.GetHelp();
  Console.WriteLine(res2);

  var res3 = mng.SendMalCommand();
  // Exception is thrown
}

// Run openvpn with login and password and connect to it
// "my_login", "my_password" will be saved in a file and its path written to "Andorra.AndorralaVella.TCP.ovpn" 
using (var mng = new Manager("127.0.0.1", 8888, "Andorra.AndorralaVella.TCP.ovpn", "my_login", "my_password")) {
  var a1 = mng.GetStatus();
  Console.WriteLine(a1);

  var a2 = mng.GetPid();
  Console.WriteLine(a2);

  var a3 = mng.GetHelp();
  Console.WriteLine(a3);
}

```

### License
Apache 2.0