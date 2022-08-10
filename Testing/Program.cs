
using System.Net.Sockets;

string server = "http://192.168.0.10:80/";
HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("User-Agent", "OI.Share v2");
//var response = await client.GetAsync(server+"get_connectmode.cgi");
//Console.WriteLine(response.StatusCode.ToString());
//Console.WriteLine(response.Headers.ToString());
//Console.WriteLine(await response.Content.ReadAsStringAsync());
//response = await client.GetAsync(server+"get_commandlist.cgi");
//Console.WriteLine(response.StatusCode.ToString());
//Console.WriteLine(response.Headers.ToString());
//Console.WriteLine(await response.Content.ReadAsStringAsync());
var response = await client.GetAsync(server+"switch_cammode.cgi?mode=rec&lvqty=1280x0960");
Console.WriteLine(response.StatusCode.ToString());
Console.WriteLine(response.Headers.ToString());
Console.WriteLine(await response.Content.ReadAsStringAsync());
response = await client.GetAsync(server+"exec_takemisc.cgi?com=startliveview&port=200");
Console.WriteLine(response.StatusCode.ToString());
Console.WriteLine(response.Headers.ToString());
Console.WriteLine(await response.Content.ReadAsStringAsync());
Console.ReadKey();
response = await client.GetAsync(server+"exec_takemisc.cgi?com=stopliveview");
Console.WriteLine(response.StatusCode.ToString());
Console.WriteLine(response.Headers.ToString());
Console.WriteLine(await response.Content.ReadAsStringAsync());