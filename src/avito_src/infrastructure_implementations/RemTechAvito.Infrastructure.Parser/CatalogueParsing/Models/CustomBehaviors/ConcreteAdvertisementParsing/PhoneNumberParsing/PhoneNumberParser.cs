using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhoneNumberParsing;

internal sealed class PhoneNumberParser : IWebDriverBehavior
{
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;
    private const string UrlPrefix = "https://m.avito.ru/api/1/items/";
    private const string UrlSuffix = "/phone?key=af0deccbgcgidddjgnvljitntccdduijhdinfgjgfjir";
    private const string name = "json";
    private const string path = ".//pre";
    private const string type = "xpath";

    public PhoneNumberParser(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        try
        {
            // if (string.IsNullOrWhiteSpace(_item.Id))
            //     return Result.Success();
            //
            // WebElementPool pool = new WebElementPool();
            // OpenPageBehavior open = new OpenPageBehavior(BuildUrl());
            // GetNewElementRetriable getPre = new GetNewElementRetriable(pool, path, type, name, 10);
            // ClearPoolBehavior clear = new ClearPoolBehavior();
            //
            // await open.Execute(publisher, ct);
            // await getPre.Execute(publisher, ct);
            // await clear.Execute(publisher, ct);
            // await Task.Delay(TimeSpan.FromSeconds(5), ct); // IMPORTANT!
            //
            // ExtractPhoneNumber(pool);

            await ExtractPhoneNumberUsingHttp();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Action} unable to parse phone. Exception: {Ex}",
                nameof(PhoneNumberParsing),
                ex.Message
            );
            return new Error("Unable to parse mobile phone.");
        }
    }

    private string BuildUrl() => string.Concat(UrlPrefix, _item.Id, UrlSuffix);

    private void ExtractPhoneNumber(WebElementPool pool)
    {
        Result<WebElement> element = pool[^1];
        if (element.IsFailure)
            return;

        ReadOnlySpan<char> textSpan = element.Value.InnerText;
        int indexOfPhoneStart = textSpan.IndexOf('+');
        if (indexOfPhoneStart == -1)
            return;

        int sliceLength = 0;
        for (int index = indexOfPhoneStart + 1; ; index++)
        {
            if (textSpan[index] == '"')
                break;
            sliceLength++;
        }

        ReadOnlySpan<char> phoneSpan = textSpan.Slice(indexOfPhoneStart + 1, sliceLength);
        _item.SellerInfo.SellerContacts = $"{phoneSpan}";
    }

    private async Task ExtractPhoneNumberUsingHttp()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
                System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
        };

        var client = new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };

        client.DefaultRequestHeaders.Add(
            "accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
        );
        client.DefaultRequestHeaders.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
        client.DefaultRequestHeaders.Add("cache-control", "max-age=0");
        client.DefaultRequestHeaders.Add(
            "cookie",
            "srv_id=m5T25bscpmLEEtwt.x687sjaqJ7-J6wuedB-oC9U_CvqBOP8BdAT_33oDqXoextf6UtSTv7ou9sqF7GObqTnc.sVbOW8qAAaeoEIz8Z8JSMKZB5bdxByLjTizDkuTFSAA=.web; u=32ti61fg.qk9yvp.afa3xwtf9xo0; _ym_uid=1714409326587612171; _ym_d=1735549697; _gcl_au=1.1.1942012409.1735549698; _ga=GA1.1.1588358604.1735549698; tmr_lvid=ca3b6e06c9e13b37f1c9627bbb52c050; tmr_lvidTS=1714409326191; adrcid=AnM6D29v1qFcaj2TW80LrQA; __zzatw-avito=MDA0dBA=Fz2+aQ==; auth=1; buyer_laas_location=635340; ma_cid=5075603831730127838; ma_id=3858095871735542060647; adrcid=AnM6D29v1qFcaj2TW80LrQA; uxs_uid=02541a10-c6ae-11ef-9926-7974c84a31b4; ma_prevVisId_3485699018=ad839b6950e8d862697c2ac2e483e06e; __upin=AZ2XyT1ArvBDkgCi9/Rm5w; _ga_9NLSMYFRV5=GS1.1.1737727536.20.1.1737727614.0.0.0; __ai_fp_uuid=b4f02a8ea0ae80e7%3A9; _buzz_aidata=JTdCJTIydmFsdWUlMjIlM0ElN0IlMjJ1ZnAlMjIlM0ElMjJBWjJYeVQxQXJ2QkRrZ0NpOSUyRlJtNXclMjIlMkMlMjJicm93c2VyVmVyc2lvbiUyMiUzQSUyMjEzMi4wJTIyJTJDJTIydHNDcmVhdGVkJTIyJTNBMTczOTEzMTMxMTE4NiU3RCUyQyUyMnBhdGglMjIlM0ElMjIlMkYlMjIlMkMlMjJkb21haW4lMjIlM0ElMjIubS5hdml0by5ydSUyMiUyQyUyMmV4cGlyZXMlMjIlM0ElMjJNb24lMkMlMjAwOSUyMEZlYiUyMDIwMjYlMjAyMCUzQTAxJTNBNTElMjBHTVQlMjIlMkMlMjJTYW1lU2l0ZSUyMiUzQSUyMkxheCUyMiU3RA==; _buzz_mtsa=JTdCJTIydmFsdWUlMjIlM0ElN0IlMjJ1ZnAlMjIlM0ElMjJhZDgzOWI2OTUwZThkODYyNjk3YzJhYzJlNDgzZTA2ZSUyMiUyQyUyMmJyb3dzZXJWZXJzaW9uJTIyJTNBJTIyMTMyLjAlMjIlMkMlMjJ0c0NyZWF0ZWQlMjIlM0ExNzM5MTMxMzEwNjQwJTdEJTJDJTIycGF0aCUyMiUzQSUyMiUyRiUyMiUyQyUyMmRvbWFpbiUyMiUzQSUyMi5tLmF2aXRvLnJ1JTIyJTJDJTIyZXhwaXJlcyUyMiUzQSUyMk1vbiUyQyUyMDA5JTIwRmViJTIwMjAyNiUyMDIwJTNBMDElM0E1MSUyMEdNVCUyMiUyQyUyMlNhbWVTaXRlJTIyJTNBJTIyTGF4JTIyJTdE; pageviewCount=2227; gMltIuegZN2COuSe=EOFGWsm50bhh17prLqaIgdir1V0kgrvN; _ga_ZJDLBTV49B=GS1.1.1739640950.7.0.1739640957.0.0.0; _ga_WW6Q1STJ8M=GS1.1.1739640950.7.0.1739640957.0.0.0; cfidsw-avito=G5kM57NDIvfrkyN+npCGYidOh4N4CBUsRQeug2jlJ+HDQfjoSPAl7VR12urBL813i4DszNYt0jjuj7AB/Gs91hhP1hlKCgQVj1ygTyJWpEyNDBN4osVVqdO7PpJ2gHjDLaEccnXdHdvqIh3hY/oi+b3FPJ0GSjuIL+lhoQ==; _ym_isad=2; acs_3=%7B%22hash%22%3A%222519d36ba1d6b3a4bd08e045fbf175fd06f869ed%22%2C%22nextSyncTime%22%3A1739881354221%2C%22syncLog%22%3A%7B%22224%22%3A1739794954221%2C%221228%22%3A1739794954221%7D%7D; luri=all; buyer_location_id=621540; adrdel=1739824568327; f=5.0c4f4b6d233fb90636b4dd61b04726f19a1c4f09e41d2b199a1c4f09e41d2b199a1c4f09e41d2b199a1c4f09e41d2b19277909827d0cb161277909827d0cb161277909827d0cb1619a1c4f09e41d2b19bb0992c943830ce0bb0992c943830ce00df103df0c26013a7b0d53c7afc06d0b2ebf3cb6fd35a0ac0df103df0c26013a8b1472fe2f9ba6b9ad42d01242e34c7968e2978c700f15b6831064c92d93c3903815369ae2d1a81d4e0d8a280d6b65f00df103df0c26013aba0ac8037e2b74f971e7cb57bbcb8e0f03c77801b122405c8b1472fe2f9ba6b91d6703cbe432bc2a71e7cb57bbcb8e0f03c77801b122405c2da10fb74cac1eab2da10fb74cac1eab2ebf3cb6fd35a0ac8b1472fe2f9ba6b9b892c6c84ad16848a9b4102d42ade879dcb5a55b9498f642b39bb5830a606c656e02c3408f50bd4493bde81ae49bef92863daca71d2d61af4525907271a6a0eb7880ce9913b94034231f57a48135c490e2415097439d404746b8ae4e81acb9fa786047a80c779d5146b8ae4e81acb9fa78df997520c04c59b6c9122eda0b0e572da10fb74cac1eabb3ae333f3b35fe91de6c39666ae9b0d7312f8fecc8ca5e543486a07687daa291; ft=\"p5PW4tIa8scfZ8Ad9Cc60BmB+s1q3Luozc5CWmYH0mPvEnivgdBMBhf9CrW/rKpdVXP5uXSvAaTvyCNdEkPxrL14QtiHV8QatTdVACNaOU9zKntanQQQbwHpsD2b2LLTKC2zhu7poQVPqZ9o/G8R68Ye3tOR+Iooet/de6NwoIONcDelpESftVabi40y81tl\"; v=1739827365; _ym_visorc=b; sx=H4sIAAAAAAAC%2F1zQy0rDQBSA4XeZdRZzMmfOpTsJRaxiW6tU3c2VINoLSqiWvLurYPEFPn7%2Bs8EYKCoGylkpB3HJMoPUUpGLZjazsxnMzPTr%2FmoLp%2B3H8r77fjosTWOKmQE7FRb1NDaGiChlpqqknpC0cCxOM3ubEmedqFeyP5%2Fo1sj%2BmO3X7QVFAKpjYzg6JF%2B5FKjRtolJpNTkMYNUiThRx%2FdrWcheeNMtMJzmf5SCd57HxmTwHKk4yU5yBe9Ca620wko2ANFEHeStf1yllz4vurtuM1xUKbbSjo0pEAlK9CUARCpc0Iu1mFIKAat3E%2FWwj7uh35HcDOvnzWr%2B7xWO428AAAD%2F%2F2oSdv94AQAA; dfp_group=9; pageviewCount=2387; ma_ss_64a8dba6-67f3-4fe4-8625-257c4adae014=1438571051739827147.113.1739828733.4; _ga_M29JC28873=GS1.1.1739827147.89.1.1739828734.58.0.0; buyer_location_id=635340; srv_id=A-VoHV5kG6upCazj.f0ANiu4oYGw4VLtUurKl6aBkDIuhUmMLQK3OxMBhj_XZfyzrPXeLZio6CSVgDqM=.GjVtwr4WbnQM1nFZ0mvYgORqps6GOpagsbds22BTkxU=.web; u=32hq8fqe.f2r5hn.1p4qiaitsl800; v=1739827365"
        );
        client.DefaultRequestHeaders.Add("priority", "u=0, i");
        client.DefaultRequestHeaders.Add(
            "sec-ch-ua",
            "\"Not A(Brand\";v=\"8\", \"Chromium\";v=\"132\", \"Google Chrome\";v=\"132\""
        );
        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
        client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
        client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
        client.DefaultRequestHeaders.Add("sec-fetch-site", "none");
        client.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
        client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36"
        );

        var response = await client.GetAsync(BuildUrl());
        var content = await response.Content.ReadAsStringAsync();
        _logger.Information("Phone response: \n {Phone response}", content);

        ReadOnlySpan<char> textSpan = content;
        int indexOfPhoneStart = textSpan.IndexOf('+');
        if (indexOfPhoneStart == -1)
            return;

        int sliceLength = 0;
        for (int index = indexOfPhoneStart + 1; ; index++)
        {
            if (textSpan[index] == '"')
                break;
            sliceLength++;
        }

        ReadOnlySpan<char> phoneSpan = textSpan.Slice(indexOfPhoneStart + 1, sliceLength);
        _item.SellerInfo.SellerContacts = $"{phoneSpan}";
    }
}
