namespace EF_Core_Console.Controller;

public class OnePlusController
{
	private readonly Browser _browser;
	private readonly IOP_API _api;

	public OnePlusController(IServiceProvider service)
	{
		_browser = service.GetRequiredService<Browser>();
		_api = service.GetRequiredService<IOP_API>();
	}

	public async Task AllPhones()
	{

		var urls = _api.GetPhoneUrls().Result;
		foreach (var url in urls)
		{
			var phone = _api.GetPhoneByUrl(url, 1500);
		}
		Console.WriteLine();
	}
}
