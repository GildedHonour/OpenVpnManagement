OpenVpnManagement
====

A library in C# for communicating with OpenVPN Management Interface

### Example
```
using (var mng = new Manager("127.0.0.1", 8888)) {
  var res1 = mng.GetPid();
  Console.WriteLine(res1);

  var res2 = mng.GetHelp();
  Console.WriteLine(res2);

  var res3 = mng.SendMalCommand();
  // Exception is thrown
}

```