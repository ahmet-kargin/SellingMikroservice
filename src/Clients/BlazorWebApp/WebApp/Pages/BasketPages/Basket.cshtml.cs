using CatalogService.Api.Core.Application.ViewModels;
using CatalogService.Api.Core.Domain;
using Consul;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Components;
using WebApp.Domain.Models.ViewModels;
using WebApp.Infrastructure;
using WebApp.Services;

public partial class Catalog : ComponentBase
{
	PaginatedItemsViewModel<CatalogItem> model = new PaginatedItemsViewModel<CatalogItem>();

	[Inject]
	ICatalogRepsository catalogService { get; set; }

	[Inject]
	IIdentityService identityService { get; set; }

	[Inject]
	public IBasketRepository basketService { get; set; }

	[Inject]
	NavigationManager navigationManager { get; set; }

	[Inject]
	AppStateManager appState { get; set; }

	protected override async Task OnInitializedAsync()
	{
		model = await catalogService.GetCatalogItems();
	}

	public async Task AddToCart(CatalogItem catalogItem)
	{
		if (!identityService.IsLoggedIn)
		{
			navigationManager.NavigateTo($"login?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}", true);
			return;
		}

		await basketService.AddItemToBasket(catalogItem.Id);
		appState.UpdateCart(this);
	}
}
