# µHTTPd .NET
Micro HTTP server library & application.

**for local development & testing only**

## Features

* high performance (async/await-based)
* painfully simple to use
* built-in static file serving 
* built-in directory listing
* built-in "delegate" handler for registering C# routine for specific route

## Quick start

1. download & reference the library
2. Add following code:

```cs
var http = new uHttpServer(); 
http.InitDefaults(@"C:\test\"); // OPTIONAL: add static file handler and directory listing handler for given root directory, add an exception handler
http.Start("http://localhost:8090/"); // start listening on localhost:8090
```

## Reference

### Custom routes actions

Instead of serving static files, we'll use C# code for generating response
```cs
var http = new uHttpServer();
http.Handlers.Add(new DelegateWithParamsHandler(@"^/hello,{name:\w+}$",
  delegate(HttpListenerRequest request,HttpListenerResponse response, Dictionary<string, string> dictionary)
  {
    response.ContentType = "text/html";                    
    return String.Format("Hi there, <b>{0}</b>", dictionary["name"]);
  }));
http.Start("http://localhost:8090/");
```

now, point your browser to http://localhost:8090/hello,mike

What you'll see is: ``Hi there, mike``

### Exception handling

Built-in ``DefaultHttpExceptionHandler`` is enabled by default when ``InitDefaults`` is called. 
The ``DefaultHttpExceptionHandler`` translates every HTTP exception into the proper HTTP status code, and displays the error header.

Optionally, stack trace displaying might be enabled:
```cs
http.ExceptionHandlers.OfType<DefaultHttpExceptionHandler>().Single().ShowStackTrace = true;
```

### To-do

Please be aware that this project is not intended to compete with production servers; instead it's supposed to be as easy and simple as possible.

* logging (NLog / log4net)
* wrap response object into custom class for more control over StatusCode and ContentType
* request & response filters 
* serving embedded resources

Features **not being considered** currently:

* authentication
* fastcgi

### License

Feel free to use under MIT license, just let me know if you find it useful.


Copyright 2014 Michał Gajek "migajek"