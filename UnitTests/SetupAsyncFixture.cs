using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Moq.Tests
{
	public class SetupAsyncFixture
	{
		public interface IBarAsync
		{
			Task<object> CloneAsync();
		}

		[Fact]
		public async Task ReturnsValueAsync()
		{
			var mock = new Mock<IBarAsync>();
			var clone = new object();

			mock.SetupAsync(x => x.CloneAsync()).Returns(clone);

			var r = await mock.Object.CloneAsync();

			Assert.Equal(clone, r);
		}
		
		public interface IFooAsync
		{
			Task SubmitAsync();
			Task SubmitAsync(string v);
		}

		[Fact]
		public async Task ExecutesCallbackWhenVoidAsyncMethodIsCalled()
		{
			var mock = new Mock<IFooAsync>();
			bool called = false;
			mock.SetupAsync(x => x.SubmitAsync()).Callback(() => called = true);

			await mock.Object.SubmitAsync();
			Assert.True(called);
		}
		
		[Fact]
		public async Task CallbackCalledWithOneArgument()
		{
			var mock = new Mock<IFooAsync>();
			string callbackArg = null;
			mock.SetupAsync(x => x.SubmitAsync(It.IsAny<string>())).Callback((string s) => callbackArg = s);

			await mock.Object.SubmitAsync("blah");
			Assert.Equal("blah", callbackArg);
		}

		[Fact]
		public async Task ThrowCalledWithOneArgument()
		{
			var mock = new Mock<IFooAsync>();
			mock.SetupAsync(x => x.SubmitAsync()).Throws<Exception>();
			Exception ex = await Assert.ThrowsAsync<Exception>(async() => await mock.Object.SubmitAsync());
		}
	}
}
