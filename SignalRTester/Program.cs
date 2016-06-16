using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Owin;

namespace SignalRTester {
	class Program {
		static void Main(string[] args) {
			using (var server = new TestWebServer()) {
				using (var hubConnection = new HubConnection(TestWebServer.Host)) {
					var proxy = hubConnection.CreateHubProxy("testHub");
					hubConnection.Start().Wait();
					Debug.Assert(hubConnection.State == ConnectionState.Connected);
				}
			}


			using (var server = new TestWebServer()) {
				using (var hubConnection = new HubConnection(TestWebServer.Host)) {
					var proxy = hubConnection.CreateHubProxy("testHub");
					hubConnection.Start().Wait();
					Debug.Assert(hubConnection.State == ConnectionState.Connected);
				}
			}

		}
	}

	[HubName("testHub")]
	public class TestHub : Hub {
		public void Ping() => Clients.All.pong();
	}
	//public class TestWebServer : BaseApiWebServer<TestHub>, IWebServer {
	//	public TestWebServer() : base("basketballApp", new WebRemoteSettings(), Messenger.Default) { }
	//}

	public class TestWebServer : IDisposable {
		public const string Host = "http://localhost:8081/";
		public TestWebServer() {
			this.server = WebApp.Start(Host, app => {
				var cfg = new HubConfiguration { };
				app.MapSignalR(cfg);
			});
		}
		IDisposable server;

		//Super-duper-for-real disposable https://lostechies.com/chrispatterson/2012/11/29/idisposable-done-right/
		bool _disposed;
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~TestWebServer() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (_disposed)
				return;
			if (disposing) {
				if (null == server) return;
				server.Dispose();
				server = null;
			}
			_disposed = true;
		}

	}

}
