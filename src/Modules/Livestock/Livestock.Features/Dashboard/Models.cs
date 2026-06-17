namespace Livestock.Features.Dashboard;

public record DashboardStats(
    int TotalProducts, int ActiveProducts, int PendingApprovalProducts,
    int TotalSellers, int VerifiedSellers,
    int TotalOffers, int PendingOffers,
    int TotalConversations, int TotalDeals,
    int TotalTransportRequests);

public record SellerDashboardStats(
    int TotalProducts, int ActiveProducts, int PendingApprovalProducts,
    int TotalViews, double AverageRating, int TotalReviews,
    int TotalOffers, int PendingOffers, int TotalDeals,
    int TotalFavorites);
