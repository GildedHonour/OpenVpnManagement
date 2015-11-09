OpenVpnManagement
====

C# OpenVPN interface

### Example
```
//Connect to the running openvpn process
using (var mng = new Manager("127.0.0.1", 8888)) {
  var res1 = mng.GetPid();
  Console.WriteLine(res1);

  var res2 = mng.GetHelp();
  Console.WriteLine(res2);

  var res3 = mng.SendMalCommand();
  // Exception is thrown
}

//Run openvpn and connect to it
using (var mng = new Manager("127.0.0.1", 8888, "Andorra.AndorralaVella.TCP.ovpn")) {
  var res1 = mng.GetPid();
  Console.WriteLine(res1);

  var res2 = mng.GetHelp();
  Console.WriteLine(res2);

  var res3 = mng.SendMalCommand();
  // Exception is thrown
}

```

### License
Apache 2.0