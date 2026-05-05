using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FoodOrdering.Cashier;

public class ApiClient
{
    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:8080")
    };

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _http.GetFromJsonAsync<List<Order>>("/api/orders")
               ?? new List<Order>();
    }

    public async Task<Order?> GetOrderAsync(long id)
    {
        return await _http.GetFromJsonAsync<Order>($"/api/orders/{id}");
    }

    public async Task<Order?> UpdateStatusAsync(long id, string status)
    {
        var request = new
        {
            status = status
        };

        var json = JsonSerializer.Serialize(request);

        var response = await _http.PatchAsync(
            $"/api/orders/{id}/status",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        return await response.Content.ReadFromJsonAsync<Order>();
    }

    public async Task<Order?> UpdatePaymentAsync(long orderId, decimal amountPaid)
    {
        var request = new
        {
            amountPaid = amountPaid
        };

        var response = await _http.PutAsJsonAsync(
            $"/api/orders/{orderId}/payment",
            request
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        return await response.Content.ReadFromJsonAsync<Order>();
    }

    public async Task<Receipt?> GetReceiptAsync(long orderId)
    {
        return await _http.GetFromJsonAsync<Receipt>($"/api/receipts/{orderId}");
    }
}