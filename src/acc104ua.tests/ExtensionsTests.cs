using Xunit;

namespace acc104ua
{
	[Trait("TestCategory", "Unittest")]
	public class ExtensionsTests
	{
		[Theory]
		[InlineData("41.45", 41.45)]
		[InlineData(" 41.45 ", 41.45)]
		[InlineData(" 41.45 грн ", 41.45)]
		[InlineData(" 41.45 \n грн ", 41.45)]
		public void Should_ParseMoney_Strip(string input, double output)
		{
			Assert.Equal(output, input.ParseMoney());
		}
	}
}
