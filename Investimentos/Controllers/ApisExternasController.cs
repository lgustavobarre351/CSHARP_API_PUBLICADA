using Microsoft.AspNetCore.Mvc;
using ProjetoInvestimentos.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ProjetoInvestimentos.Controllers;

/// <summary>
/// APIs externas e validações
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("3️⃣ APIS EXTERNAS - B3, HG Brasil e validações externas")]
public class ApisExternasController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IB3ValidationService _b3ValidationService;
    private readonly IConfiguration _configuration;

    public ApisExternasController(HttpClient httpClient, IB3ValidationService b3ValidationService, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _b3ValidationService = b3ValidationService;
        _configuration = configuration;
    }

    /// <summary>
    /// Lista códigos de ações da B3 com cotação disponível (LINQ)
    /// </summary>
    /// <returns>Lista de códigos B3 com cotação na API</returns>
    [HttpGet("codigos-b3")]
    [SwaggerOperation(
        Summary = "Lista códigos de ações da B3 com cotação disponível [LINQ]",
        Description = "🔍 CONSULTA LINQ: Retorna apenas ações que possuem cotação disponível na API Brapi. Lista curada e testada de ações líquidas da B3."
    )]
    [SwaggerResponse(200, "Lista de códigos da B3 com cotação", typeof(object))]
    public async Task<ActionResult<object>> GetB3Codes()
    {
        try
        {
            // Lista curada de ações com alta liquidez que SEMPRE têm cotação disponível na Brapi
            var stocksWithQuotes = new List<string>
            {
                // Bancos
                "ITUB4", "BBDC4", "BBAS3", "SANB11", "BPAC11", "BBDC3", "ITUB3",
                
                // Petróleo e Energia
                "PETR3", "PETR4", "VALE3", "PRIO3", "CSAN3", "RRRP3", "RECV3", "CPLE6",
                
                // Varejo
                "MGLU3", "LREN3", "ABEV3", "ASAI3", "PCAR3", "BTOW3", "AMER3", "VIVA3", "SOMA3",
                
                // Tecnologia
                "B3SA3", "TOTS3", "LWSA3", "POSI3",
                
                // Construção e Imobiliário
                "MRVE3", "CYRE3", "MULT3", "EZTC3", "TEND3", "JHSF3",
                
                // Industrial
                "WEGE3", "RAIL3", "RENT3", "SUZB3", "KLBN11", "EMBR3", "RAIZ4",
                
                // Siderurgia e Mineração
                "CSNA3", "GGBR4", "USIM5", "GOAU4", "VALE5",
                
                // Utilities
                "ELET3", "ELET6", "CMIG4", "SBSP3", "TAEE11", "EGIE3", "CPLE3", "CPFE3", "SAPR11",
                
                // Saúde
                "HAPV3", "QUAL3", "RDOR3", "FLRY3", "DASA3",
                
                // Educação
                "COGN3", "YDUQ3",
                
                // Alimentos
                "JBSS3", "MRFG3", "BEEF3", "SMTO3", "BRFS3", "SLCE3",
                
                // Telecomunicações
                "VIVT3", "TIMS3",
                
                // Farmácias
                "RADL3", "PNVL3",
                
                // Logística
                "LOGN3", "CCRO3",
                
                // Aviação
                "AZUL4", "GOLL4",
                
                // Papel e Celulose
                "SUZB3", "KLBN11",
                
                // Outros importantes
                "NTCO3", "IRBR3", "CASH3", "CVCB3", "IGTI11", "BBSE3",
                "AZZA3", "BRML3", "CRFB3", "CSMG3", "DIRR3", "ENGI11",
                "EQTL3", "GMAT3", "HYPE3", "INTB3", "ITSA4", "KLBN4",
                "LAME4", "LIGT3", "MDIA3", "MILS3", "MOVI3", "ODPV3",
                "PETZ3", "POMO4", "PRIO3", "PSSA3", "RANI3", "RAPT4",
                "SBFG3", "SHOW3", "SIMH3", "SMFT3", "STBP3", "TAEE4",
                "TIMS3", "TOTS3", "UGPA3", "USIM3", "VAMO3", "VIIA3",
                "VIVT4", "VULC3", "WIZC3", "YDUQ3"
            };

            // LINQ: Where para filtrar apenas códigos que também estão na lista do serviço B3
            var allB3Codes = await _b3ValidationService.GetAllB3CodesAsync();
            var validatedStocks = stocksWithQuotes
                .Where(code => allB3Codes.Contains(code)) // LINQ: Where + Contains
                .OrderBy(code => code) // LINQ: OrderBy para ordenar alfabeticamente
                .ToList();

            return Ok(new
            {
                Success = true,
                Message = "Códigos B3 com cotação disponível (ações líquidas)",
                Data = new
                {
                    TotalValidadas = validatedStocks.Count, // LINQ: Count
                    TotalB3 = allB3Codes.Count,
                    Codes = validatedStocks.Take(50), // LINQ: Take para primeiros 50
                    AllCodes = validatedStocks // Lista completa ordenada
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Erro = "Erro ao obter códigos B3",
                Detalhes = ex.Message
            });
        }
    }

    /// <summary>
    /// Recarrega códigos da B3 (LINQ)
    /// </summary>
    /// <returns>Status da operação</returns>
    [HttpPost("recarregar-b3")]
    [SwaggerOperation(
        Summary = "Recarrega códigos da B3 [LINQ]",
        Description = "🔍 CONSULTA LINQ: Usa Count() para contagem após recarregamento. Força o recarregamento da lista de códigos da B3"
    )]
    public async Task<ActionResult<object>> ReloadB3Codes()
    {
        try
        {
            await _b3ValidationService.ReloadAsync();
            var codes = await _b3ValidationService.GetAllB3CodesAsync();
            
            return Ok(new
            {
                Success = true,
                Message = "Códigos B3 recarregados com sucesso",
                TotalCodes = codes.Count() // LINQ: Count para total de códigos recarregados
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Erro = "Erro ao recarregar códigos B3",
                Detalhes = ex.Message
            });
        }
    }

    /// <summary>
    /// Consulta cotação de uma ação
    /// </summary>
    /// <param name="symbol">Código da ação</param>
    /// <returns>Cotação atual</returns>
    [HttpGet("cotacao/{symbol}")]
    [SwaggerOperation(
        Summary = "Cotação atual de ação",
        Description = "Consulta a cotação atual de uma ação específica usando Yahoo Finance (API gratuita, sem necessidade de chave). Retorna preço, variação, volume e dados de mercado."
    )]
    [SwaggerResponse(200, "Cotação obtida com sucesso", typeof(object))]
    [SwaggerResponse(400, "Erro na consulta da cotação")]
    [SwaggerResponse(404, "Ação não encontrada")]
    public async Task<ActionResult<object>> GetStockQuote([Required] string symbol)
    {
        try
        {
            // Validar se o código existe na B3
            var isValid = await _b3ValidationService.IsB3StockAsync(symbol.ToUpper());
            if (!isValid)
            {
                return NotFound(new
                {
                    Success = false,
                    Erro = "Código de ação não encontrado na B3",
                    Symbol = symbol.ToUpper(),
                    Message = "Verifique se o código está correto usando /api/ApisExternas/codigos-b3"
                });
            }

            // Usar Yahoo Finance via query2.finance.yahoo.com - API GRATUITA
            // Para ações brasileiras da B3, adicionar .SA no final do símbolo
            var yahooSymbol = $"{symbol.ToUpper()}.SA";
            var url = $"https://query2.finance.yahoo.com/v8/finance/chart/{yahooSymbol}?interval=1d&range=1d";

            // Criar requisição com headers adequados
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new
                {
                    Success = false,
                    Erro = "Erro ao consultar cotação externa",
                    StatusCode = (int)response.StatusCode,
                    Message = $"Não foi possível obter cotação da ação {symbol.ToUpper()}",
                    Details = errorContent
                });
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            // Estrutura do Yahoo Finance: chart.result[0]
            if (!json.TryGetProperty("chart", out var chart) ||
                !chart.TryGetProperty("result", out var results) ||
                results.ValueKind != JsonValueKind.Array ||
                results.GetArrayLength() == 0)
            {
                return NotFound(new
                {
                    Success = false,
                    Erro = "Ação não encontrada",
                    Symbol = symbol.ToUpper(),
                    Message = "A API Yahoo Finance não retornou dados para este símbolo"
                });
            }

            var result = results[0];
            var meta = result.GetProperty("meta");
            var indicators = result.GetProperty("indicators");
            var quote = indicators.GetProperty("quote")[0];

            // Extrair preços
            decimal regularMarketPrice = 0m;
            decimal previousClose = 0m;
            decimal open = 0m;
            decimal dayHigh = 0m;
            decimal dayLow = 0m;
            long volume = 0L;

            if (meta.TryGetProperty("regularMarketPrice", out var priceEl) && priceEl.ValueKind == JsonValueKind.Number)
                regularMarketPrice = priceEl.GetDecimal();
            
            if (meta.TryGetProperty("chartPreviousClose", out var prevEl) && prevEl.ValueKind == JsonValueKind.Number)
                previousClose = prevEl.GetDecimal();

            // Obter arrays de dados (último valor de cada)
            if (quote.TryGetProperty("open", out var openArr) && openArr.ValueKind == JsonValueKind.Array && openArr.GetArrayLength() > 0)
            {
                var lastOpen = openArr[openArr.GetArrayLength() - 1];
                if (lastOpen.ValueKind == JsonValueKind.Number)
                    open = lastOpen.GetDecimal();
            }

            if (quote.TryGetProperty("high", out var highArr) && highArr.ValueKind == JsonValueKind.Array && highArr.GetArrayLength() > 0)
            {
                var lastHigh = highArr[highArr.GetArrayLength() - 1];
                if (lastHigh.ValueKind == JsonValueKind.Number)
                    dayHigh = lastHigh.GetDecimal();
            }

            if (quote.TryGetProperty("low", out var lowArr) && lowArr.ValueKind == JsonValueKind.Array && lowArr.GetArrayLength() > 0)
            {
                var lastLow = lowArr[lowArr.GetArrayLength() - 1];
                if (lastLow.ValueKind == JsonValueKind.Number)
                    dayLow = lastLow.GetDecimal();
            }

            if (quote.TryGetProperty("volume", out var volArr) && volArr.ValueKind == JsonValueKind.Array && volArr.GetArrayLength() > 0)
            {
                var lastVol = volArr[volArr.GetArrayLength() - 1];
                if (lastVol.ValueKind == JsonValueKind.Number)
                    volume = lastVol.GetInt64();
            }

            // Calcular variação
            var variacao = regularMarketPrice - previousClose;
            var variacaoPercent = previousClose > 0 ? (variacao / previousClose) * 100 : 0;

            // Montar objeto de resposta
            var cotacao = new
            {
                Simbolo = symbol.ToUpper(),
                Nome = meta.TryGetProperty("longName", out var nameEl) ? nameEl.GetString() : 
                       meta.TryGetProperty("shortName", out var shortNameEl) ? shortNameEl.GetString() : symbol.ToUpper(),
                Preco = regularMarketPrice,
                Variacao = variacao,
                VariacaoPercent = Math.Round(variacaoPercent, 2),
                Abertura = open,
                MinimoDia = dayLow,
                MaximoDia = dayHigh,
                FechamentoAnterior = previousClose,
                Volume = volume,
                Moeda = meta.TryGetProperty("currency", out var currEl) ? currEl.GetString() : "BRL",
                Exchange = meta.TryGetProperty("exchangeName", out var exEl) ? exEl.GetString() : "B3"
            };

            return Ok(new
            {
                Success = true,
                Symbol = symbol.ToUpper(),
                Message = "Cotação obtida com sucesso",
                Data = cotacao,
                Timestamp = DateTime.UtcNow,
                Source = "Yahoo Finance (Gratuita)"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Erro = "Erro ao obter cotação",
                Symbol = symbol,
                Detalhes = ex.Message
            });
        }
    }

    /// <summary>
    /// Valida código de ação
    /// </summary>
    /// <param name="symbol">Código para validar</param>
    /// <returns>Resultado da validação</returns>
    [HttpGet("validar-codigo/{symbol}")]
    [SwaggerOperation(
        Summary = "Validar código de ação B3",
        Description = "Verifica se um código de ação é válido na B3"
    )]
    public async Task<ActionResult<object>> ValidateB3Code([Required] string symbol)
    {
        try
        {
            var isValid = await _b3ValidationService.IsB3StockAsync(symbol.ToUpper());
            
            return Ok(new
            {
                Success = true,
                Symbol = symbol.ToUpper(),
                IsValid = isValid,
                Message = isValid ? "Código válido na B3" : "Código não encontrado na B3"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Erro = "Erro ao validar código",
                Symbol = symbol,
                Detalhes = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtém preço atual do Bitcoin
    /// </summary>
    /// <returns>Preço do Bitcoin em BRL e USD</returns>
    /// <response code="200">Preço obtido com sucesso</response>
    /// <response code="500">Erro ao consultar preço</response>
    [HttpGet("bitcoin-price")]
    [SwaggerOperation(
        Summary = "Consulta preço do Bitcoin",
        Description = "Conecta com a API CoinGecko para obter o preço atual do Bitcoin em Real Brasileiro e Dólar Americano"
    )]
    [SwaggerResponse(200, "Preço do Bitcoin obtido", typeof(object))]
    [SwaggerResponse(500, "Erro ao consultar preço do Bitcoin")]
    public async Task<ActionResult<object>> GetBitcoinPrice()
    {
        try
        {
            // API pública do CoinGecko
            var url = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=brl,usd";
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    Success = false,
                    Erro = "Erro ao consultar API CoinGecko",
                    StatusCode = (int)response.StatusCode
                });
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(content);

            return Ok(new
            {
                Success = true,
                Message = "Preço do Bitcoin obtido com sucesso",
                Data = new
                {
                    Bitcoin = data,
                    ConsultadoEm = DateTime.UtcNow,
                    Fonte = "CoinGecko API"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Erro = "Erro interno ao consultar Bitcoin",
                Detalhes = ex.Message
            });
        }
    }
}