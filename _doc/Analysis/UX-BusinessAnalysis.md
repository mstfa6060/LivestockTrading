# GlobalLivestock Platform - UX & Business Analysis

**Date:** 2026-02-11
**Scope:** Backend API endpoint inventory, feature gap analysis, user journey assessment, and competitor benchmarking for the GlobalLivestock agricultural marketplace platform.

---

## 1. Executive Summary

GlobalLivestock is an ambitious multi-country, multi-language agricultural marketplace that goes well beyond basic livestock trading. The platform covers **live animals, seeds, feed, chemicals, veterinary products, and farm machinery** -- a broad agricultural B2B/B2C scope. The backend is architecturally mature, built on a modular monolith with clean separation of concerns, real-time messaging (SignalR), background workers (RabbitMQ), and a robust RBAC system.

**Current state:** The platform has a solid foundation of ~35+ entity types, ~130+ API endpoints, and infrastructure for multi-country, multi-language, and multi-currency operation. However, several **business-critical** user flows remain incomplete or missing entirely, which would block a production launch. The most significant gaps are: **no Order/Payment processing pipeline, no escrow/payment gateway integration, no auction/bidding system, incomplete dispute resolution, and missing advanced search capabilities** (full-text search, geospatial proximity).

**Verdict:** The platform is approximately **60-65% complete** for a minimum viable marketplace. The remaining 35-40% involves high-complexity features (payment, orders, escrow, compliance) that are essential for user trust and revenue generation.

---

## 2. Current Feature Inventory

### 2.1 IAM Module (Identity & Access Management)

| Endpoint Group | Endpoints | Status |
|---|---|---|
| Auth/Login | Native email/password login | Complete |
| Auth/RefreshToken | JWT refresh flow | Complete |
| Auth/Logout | Token revocation | Complete |
| Auth/SendOtp, VerifyOtp | OTP-based authentication | Complete |
| Users/Create | Registration with auto Buyer role | Complete |
| Users/Update | Profile updates | Complete |
| Users/Detail | User profile retrieval | Complete |
| Users/ForgotPassword, ResetPassword | Password recovery | Complete |
| Users/Delete | Account deletion | Complete |
| Countries/All | Country listing (196 countries) | Complete |
| Provinces, Districts, Neighborhoods | Turkey address hierarchy | Complete |
| Push/RegisterToken | Push notification token registration | Complete |
| Role CRUD | Admin role management | Complete |

**Assessment:** IAM is feature-complete for MVP. Social login (Google, Apple) is configured but needs verification. Missing: email verification flow, account deactivation (vs hard delete), 2FA/MFA.

### 2.2 LivestockTrading Module - Core Entities

#### Product Management (11 endpoints)

| Endpoint | Purpose | Status |
|---|---|---|
| Products/Create | Create new listing | Complete |
| Products/Update | Edit listing | Complete |
| Products/Delete | Soft delete | Complete |
| Products/All | Paginated listing with filters | Complete |
| Products/Detail | Single product by ID | Complete |
| Products/DetailBySlug | SEO-friendly detail | Complete |
| Products/Pick | Dropdown selection | Complete |
| Products/Search | Advanced search with filters | Complete |
| Products/Approve | Moderator approval | Complete |
| Products/Reject | Moderator rejection | Complete |
| Products/MediaDetail | Media bucket retrieval | Complete |

**Search filters available:** Query (title/description/slug), CategoryId, BrandId, MinPrice, MaxPrice, Condition, CountryCode, City, SellerId, Sorting, Pagination.

**Missing search capabilities:** Breed/animal type filter, proximity/radius search, seller rating filter, organic/certified filter, multi-category filter, saved searches, full-text search (currently LIKE-based).

#### Product Type-Specific Information (6 sub-entity groups)

| Entity | Endpoints | Status |
|---|---|---|
| AnimalInfos | Create, All, Pick | Partial (missing Update, Delete, Detail) |
| FeedInfos | Create, All, Pick | Partial |
| ChemicalInfos | Create, All, Pick | Partial |
| MachineryInfos | Create, All, Pick | Partial |
| SeedInfos | Create, All, Pick, Update, Delete, Detail | Complete |
| VeterinaryInfos | Create, All, Pick, Update, Delete, Detail | Complete |

**Assessment:** AnimalInfos, FeedInfos, ChemicalInfos, and MachineryInfos are missing Update, Delete, and Detail endpoints, limiting sellers' ability to manage these critical product details.

#### Health & Vaccination Records

| Entity | Endpoints | Status |
|---|---|---|
| HealthRecords | Create, All, Pick | Partial (missing Update, Delete, Detail) |
| Vaccinations | Create, All, Pick, Update, Delete, Detail | Complete |

#### Seller Management (9 endpoints)

| Endpoint | Purpose | Status |
|---|---|---|
| Sellers/Create | Create seller profile (auto on product create) | Complete |
| Sellers/Update | Update profile | Complete |
| Sellers/Delete | Soft delete | Complete |
| Sellers/All | List sellers | Complete |
| Sellers/Detail | Seller profile by ID | Complete |
| Sellers/DetailByUserId | Seller profile by user | Complete |
| Sellers/Pick | Dropdown | Complete |
| Sellers/Verify | Moderator verification | Complete |
| Sellers/Suspend | Moderator suspension | Complete |

#### Transporter Management (8 endpoints)

| Endpoint | Purpose | Status |
|---|---|---|
| Transporters/Create-Delete-Update | Full CRUD | Complete |
| Transporters/All-Detail-Pick | Queries | Complete |
| Transporters/Verify | Moderator verification | Complete |
| Transporters/Suspend | Moderator suspension | Complete |

#### Transport Workflow

| Entity | Endpoints | Status |
|---|---|---|
| TransportRequests | Full CRUD + Detail | Complete |
| TransportOffers | Full CRUD + Detail | Complete |
| TransportTrackings | Full CRUD + Detail | Complete |

**Assessment:** Transport workflow entities exist and have full CRUD, but the **orchestration layer** (accepting an offer updates request status, GPS tracking integration, ETA calculations, route optimization) appears to be manual data entry rather than automated workflow.

#### Deal Management (6 endpoints)

| Endpoint | Purpose | Status |
|---|---|---|
| Deals/Create-Update-Delete | Commands | Complete |
| Deals/All-Detail-Pick | Queries | Complete |

**Assessment:** The Deal entity captures agreement details (price, quantity, delivery method, status tracking) but operates as a **record-keeping system** rather than a transactional workflow. There are no automated state transitions, no payment integration, and no notification triggers on status changes.

#### Messaging System

| Endpoint | Purpose | Status |
|---|---|---|
| Conversations/All-Detail-Delete-Update-Pick | Conversation management | Complete |
| Messages/Create-Update-Delete | Message operations | Complete |
| Messages/All-Detail-Pick | Message queries | Complete |
| Messages/SendTypingIndicator | Real-time typing | Complete |
| Messages/UnreadCount | Badge count | Complete |
| SignalR Hub (/hubs/chat) | Real-time WebSocket | Complete |

**Assessment:** Messaging is the most complete feature area. Real-time delivery, typing indicators, read receipts, and presence detection are all implemented. Missing: message search, file/image sharing via messages (entity supports AttachmentUrls but no upload flow), message reporting/spam detection.

#### Offer/Negotiation System

| Endpoint | Purpose | Status |
|---|---|---|
| Offers/Create-Update-Delete | Offer management | Complete |
| Offers/All-Detail-Pick | Offer queries | Complete |

**Assessment:** Supports offers, counter-offers, and status tracking. Missing: automatic expiry enforcement, notification on offer events, conversion from accepted offer to Deal.

#### Reviews & Ratings

| Entity | Endpoints | Status |
|---|---|---|
| ProductReviews | Create, Update, Delete, All, Pick | Complete (missing Detail) |
| SellerReviews | Full CRUD + Detail | Complete |
| TransporterReviews | Full CRUD + Detail | Complete |

**Assessment:** Three-dimensional review system covering products, sellers, and transporters. Supports multi-dimensional ratings (communication, quality, timeliness). Missing: review moderation workflow (approve/reject endpoints), helpful vote endpoints, average rating recalculation triggers.

#### Supporting Entities

| Entity | Endpoints | Status |
|---|---|---|
| Categories | Create, Update, Delete, All, Detail, Pick | Complete (with i18n) |
| Brands | Full CRUD + Detail | Complete |
| Currencies | Full CRUD + Detail | Complete |
| Languages | Full CRUD + Detail | Complete |
| Locations | Full CRUD + Detail | Complete |
| Farms | Full CRUD + Detail | Complete |
| FAQs | Create, Update, Delete, All, Pick | Complete (with i18n) |
| Banners | Create, Update, Delete, All, Pick | Complete |
| PaymentMethods | Full CRUD + Detail | Complete |
| ProductPrices | Create, All, Pick | Partial (missing Update, Delete, Detail) |
| ProductVariants | Create, All, Pick, Update, Delete, Detail | Complete |
| ShippingZones | Full CRUD + Detail | Complete |
| ShippingRates | Full CRUD + Detail | Complete |
| ShippingCarriers | Full CRUD + Detail | Complete |
| TaxRates | Full CRUD + Detail | Complete |
| FavoriteProducts | Create, All, Pick | Partial (missing Delete/Toggle) |
| Notifications | Create, All, Pick | Partial (missing mark-as-read) |
| UserPreferences | Full CRUD + Detail | Complete |
| Dashboard/Stats | Seller dashboard metrics | Complete |

### 2.3 Workers (Background Services)

| Worker | Module | Status |
|---|---|---|
| MailSender | IAM | Complete |
| SmsSender | IAM | Complete |
| MailSender | LivestockTrading | Complete |
| SmsSender | LivestockTrading | Complete |
| NotificationSender | LivestockTrading | Complete |

### 2.4 Infrastructure

| Capability | Status |
|---|---|
| Multi-country (196 countries, ISO codes) | Complete |
| Multi-language (Language entity, NameTranslations JSON) | Complete |
| Multi-currency (Currency entity, exchange rates) | Complete |
| Geolocation (Location entity with lat/lng) | Complete |
| File storage (MinIO/S3 via FileProvider) | Complete |
| Caching (Redis L2 + Memory L1) | Complete |
| Real-time (SignalR) | Complete |
| Background jobs (RabbitMQ + Hangfire) | Complete |
| API Gateway (Ocelot) | Complete |
| SEO (slug-based routes, meta fields) | Complete |

---

## 3. Gap Analysis - Missing Features

### 3.1 CRITICAL - Must Have Before Launch

#### 3.1.1 Order Management System (NOT IMPLEMENTED)

**Impact:** Without orders, the platform cannot facilitate transactions. Currently, the Deal entity serves as a lightweight agreement record, but there is no:

- **Order entity** with lifecycle (Placed -> Confirmed -> Processing -> Shipped -> Delivered -> Completed)
- **Order line items** supporting multi-product orders
- **Order numbering** and tracking
- **Invoice generation**
- **Order cancellation** with refund workflow
- **Order history** for buyers and sellers
- **Automated stock decrement** on order placement
- **Order-based review gating** (only verified purchasers can review)

**Entity exists in code reference:** The `Deal` entity has `DealStatus` enum covering Agreed -> AwaitingPayment -> Paid -> Preparing -> InDelivery -> Delivered -> Completed. However, this is a bilateral agreement tracker, not a proper order system.

**Recommendation:** Create a full Order/OrderItem entity system with automated status transitions triggered by payment and shipping events.

#### 3.1.2 Payment Processing (NOT IMPLEMENTED)

**Impact:** No mechanism for actual money transfer between buyers and sellers.

Missing:
- **Payment gateway integration** (Stripe, PayPal, local gateways for multi-country)
- **Payment entity** tracking payment status, amounts, fees
- **Escrow system** to hold funds until delivery is confirmed
- **Commission/platform fee** calculation and collection
- **Refund processing**
- **Payment dispute** handling
- **Multi-currency payment** (buyer pays in their currency, seller receives in theirs)
- **Bank transfer verification** (for manual payment methods)

The `PaymentMethod` entity exists as a catalog/configuration entity, but there are no transaction entities.

**Recommendation:** Implement Payment and PaymentTransaction entities with integration to at least one payment gateway (Stripe recommended for multi-currency). Implement escrow pattern where platform holds funds.

#### 3.1.3 Advanced Search & Discovery (PARTIALLY IMPLEMENTED)

**Impact:** Current search (Products/Search) provides basic text matching and filters. For an agricultural marketplace, users need:

Missing:
- **Full-text search** with relevance ranking (current implementation is LIKE-based SQL)
- **Geospatial proximity search** ("animals within 50km of me") -- Location entity has lat/lng but no spatial queries
- **Breed/species search** for animals (requires joining AnimalInfo)
- **Multi-faceted filtering** (combine breed + age + weight + location)
- **Category tree navigation** (browse Livestock > Cattle > Dairy Cows)
- **Saved searches with alerts** ("notify me when new Holstein cows are listed in my area")
- **Recently viewed products** (entity exists: ProductViewHistory, but no endpoint)
- **Search suggestions/autocomplete**
- **"Similar products" recommendations**
- **Search history** (entity exists: SearchHistory, but no endpoint)

**Recommendation:** Implement Elasticsearch or similar for full-text search. Add spatial queries using NetTopologySuite (already a dependency). Create endpoints for SearchHistory and ProductViewHistory.

#### 3.1.4 FavoriteProducts - Incomplete (PARTIALLY IMPLEMENTED)

**Impact:** Users can add favorites but cannot remove them.

Missing:
- **Delete/Toggle endpoint** for removing favorites
- **FavoriteProducts/Detail** to check if a specific product is favorited

**Recommendation:** Add Delete endpoint and a "check if favorited" query.

### 3.2 HIGH PRIORITY - Required for Trust & Growth

#### 3.2.1 Dispute Resolution System (NOT IMPLEMENTED)

**Impact:** Agricultural transactions involve high-value goods (a cow can cost $2,000-$50,000). Without dispute resolution:

Missing:
- **Dispute entity** (reason, evidence, status, resolution)
- **Dispute creation** by buyer or seller
- **Evidence upload** (photos/videos of received goods vs. listing)
- **Moderator review** workflow
- **Resolution options** (refund, partial refund, replacement, dismiss)
- **Dispute history** for trust scoring
- **Escalation to platform admin**

**Recommendation:** Create Dispute entity with full workflow. This is essential for any marketplace involving live animals where condition at delivery matters greatly.

#### 3.2.2 Animal Health Certificate Integration (PARTIALLY IMPLEMENTED)

**Impact:** Legal requirement in most countries for transporting live animals.

What exists:
- HealthRecord entity with vet details, diagnosis, treatment
- Vaccination entity with certificates
- DocumentUrl fields for file uploads

What's missing:
- **Health certificate template generation** (PDF)
- **Veterinary authority verification** integration
- **Certificate validity checking** (expiry dates, required vaccinations per country)
- **Automated compliance checking** per destination country
- **Transport permit** generation for cross-border movements
- **Quarantine status** tracking

**Recommendation:** Build a compliance service that validates animal documentation requirements per destination country before allowing transport request creation.

#### 3.2.3 Notification System Enhancement (PARTIALLY IMPLEMENTED)

What exists:
- Notification entity with 13 notification types
- Push notification worker
- Email and SMS workers

What's missing:
- **Notifications/MarkAsRead** endpoint
- **Notifications/MarkAllAsRead** endpoint
- **Notification preferences granularity** (per-type opt-in/out -- UserPreferences has global toggles only)
- **Price drop alerts** (NotificationType.PriceDropAlert exists but no trigger)
- **Back-in-stock alerts** (NotificationType.ProductBackInStock exists but no trigger)
- **Saved search notifications** ("new listings matching your search")
- **Offer/Deal status change** notifications
- **Transport status update** push notifications

**Recommendation:** Add MarkAsRead endpoints. Implement event-driven notification triggers in handlers for price changes, stock updates, and search matches.

#### 3.2.4 Seller Verification Enhancement (PARTIALLY IMPLEMENTED)

What exists:
- Seller.IsVerified, VerifiedAt fields
- Sellers/Verify and Sellers/Suspend moderator endpoints
- TaxNumber, RegistrationNumber fields

What's missing:
- **Document upload for verification** (business license, tax certificate, farm registration)
- **Verification workflow** (submitted -> under review -> approved/rejected)
- **Verification badge tiers** (Basic, Verified, Premium)
- **Automatic document verification** (OCR/API integration)
- **Periodic re-verification** (annual license renewals)
- **Seller performance scoring** algorithm
- **Seller analytics** (conversion rate, response time, fulfillment rate)

**Recommendation:** Implement a SellerVerificationRequest entity with document upload support and multi-step approval workflow.

### 3.3 MEDIUM PRIORITY - Competitive Differentiation

#### 3.3.1 Auction/Bidding System (NOT IMPLEMENTED)

**Impact:** Auctions are a traditional and highly effective mechanism for livestock sales, especially for premium breeding stock.

Missing:
- **Auction entity** (start time, end time, starting price, reserve price, bid increment)
- **Bid entity** (bidder, amount, timestamp)
- **Real-time bidding** via SignalR (infrastructure exists)
- **Auto-bid / proxy bidding** support
- **Auction catalog** browsing
- **Auction notifications** (outbid alerts, ending soon, won)
- **Post-auction settlement** (winner -> order creation)
- **Auction calendar** by category/location

**Recommendation:** Leverage existing SignalR infrastructure. Create Auction and Bid entities. This feature would be a major differentiator.

#### 3.3.2 Seller Storefront / Public Profile (NOT IMPLEMENTED)

**Impact:** Sellers need a public-facing page.

Missing:
- **Seller public profile page** endpoint (products, reviews, farm info, certifications)
- **Seller follow** functionality
- **Seller comparison**
- **Seller catalog** (all products from one seller with filters)
- **Seller story/about** rich content

The Sellers/Detail endpoint exists but is not designed as a public storefront. It lacks aggregated data (product count by category, rating breakdown, response time stats).

**Recommendation:** Create a Sellers/PublicProfile query that aggregates seller data, products, reviews, and farms into a single response.

#### 3.3.3 Cart / Wishlist (NOT IMPLEMENTED)

**Impact:** No shopping cart exists. For bulk agricultural purchases, buyers may want to:

Missing:
- **Cart entity** (CartItem with product, variant, quantity)
- **Cart persistence** (logged-in users)
- **Cart → Order conversion**
- **Quantity validation** against stock
- **Price lock / reservation** period

Note: FavoriteProducts serves as a basic wishlist, but lacks removal capability.

**Recommendation:** Given that agricultural transactions are often single high-value items negotiated individually, a cart may be lower priority than the offer/deal workflow. However, for consumables (feed, chemicals, seeds), cart functionality is important.

#### 3.3.4 Reporting & Analytics (PARTIALLY IMPLEMENTED)

What exists:
- Dashboard/Stats endpoint (seller-focused: listings, views, favorites, messages, sales, revenue)
- ViewCount, FavoriteCount on products
- Banner impression/click tracking

What's missing:
- **Admin dashboard** (platform-wide stats: GMV, active users, listing growth, transaction volume)
- **Category analytics** (trending categories, price trends over time)
- **Market price index** by animal type/breed/region
- **Seasonal trend** analysis
- **Buyer analytics** (purchase history, preferences, recommended products)
- **Export to CSV/Excel** for reports
- **Real-time analytics** dashboard

**Recommendation:** Implement admin analytics endpoint. Market price index would be a strong differentiator for the agricultural sector.

#### 3.3.5 Content Management / Blog (NOT IMPLEMENTED)

Missing:
- **Blog/Article entity** for agricultural news, guides, best practices
- **Category-linked content** (e.g., "How to buy cattle" linked to cattle category)
- **SEO content pages**

**Recommendation:** Low priority for MVP but important for SEO and user education.

---

## 4. User Journey Pain Points

### 4.1 Buyer Journey

```
Register -> Browse/Search -> View Product -> Contact Seller -> Make Offer -> ???
```

**Pain Point 1: Dead-end after offer acceptance.** When a seller accepts an offer, there is no automated next step. No order is created, no payment is requested, no delivery arrangement is triggered. The buyer and seller must coordinate entirely through messaging.

**Pain Point 2: Limited discovery.** A buyer looking for "Holstein dairy cows under 3 years old within 100km" cannot perform this search. They must browse all products, then manually check animal details.

**Pain Point 3: No trust signals in search results.** Product search results include rating and review count but not seller verification status, response time, or completion rate.

**Pain Point 4: No way to compare products.** No comparison endpoint to view multiple products side-by-side with their animal/machinery specific details.

**Pain Point 5: Cannot track order/delivery.** Even though TransportTracking exists, there's no buyer-facing endpoint to track their specific order's delivery status.

### 4.2 Seller Journey

```
Register -> Create Seller Profile -> Get Verified -> List Products -> Receive Offers -> ???
```

**Pain Point 1: Complex product creation.** Creating a complete animal listing requires: Create Product -> Create AnimalInfo -> Create HealthRecords -> Create Vaccinations -> Upload Media. These are all separate API calls with no transactional guarantee.

**Pain Point 2: No inventory management.** Stock is a simple integer field. No stock reservation, no low-stock alerts, no batch management for consumables.

**Pain Point 3: No revenue tracking.** Dashboard/Stats shows TotalSales and Revenue but these appear to be counter fields on the Seller entity rather than calculated from actual transactions (no Order entity exists).

**Pain Point 4: Missing AnimalInfo management.** AnimalInfos lacks Update and Delete endpoints, so sellers cannot correct animal details after initial creation.

### 4.3 Transporter Journey

```
Register -> Create Profile -> Browse Transport Pool -> Make Offer -> Get Assigned -> ???
```

**Pain Point 1: No "transport pool" browse endpoint.** TransportRequests/All exists but likely returns all requests rather than a filtered pool of open requests in the transporter's service area.

**Pain Point 2: No route optimization.** Transporters cannot see multiple nearby requests to plan efficient multi-stop routes.

**Pain Point 3: No GPS tracking integration.** TransportTracking exists but requires manual coordinate entry. No mobile app integration for automatic location updates.

### 4.4 Admin/Moderator Journey

**Pain Point 1: No admin analytics dashboard.** The only dashboard is seller-focused.

**Pain Point 2: Review moderation.** ProductReviews and SellerReviews have IsApproved fields but no dedicated moderation queue endpoint.

**Pain Point 3: No user reporting.** No mechanism for users to report suspicious listings, sellers, or messages.

---

## 5. Competitor Comparison

### 5.1 Livestock-Specific Marketplaces

| Feature | GlobalLivestock | Livestock1 (Australia) | Cattle.com (USA) | Agriline (EU) |
|---|---|---|---|---|
| Multi-species support | Yes (6 product types) | Cattle/sheep/horses | Cattle only | Machinery + livestock |
| Health certificates | Partial (data entry) | Integrated with NLIS | Basic | Basic |
| Auction/bidding | Missing | Core feature | Core feature | Missing |
| Payment processing | Missing | Integrated | External | Integrated |
| Transport integration | Entities exist, no automation | Integrated | External | Integrated |
| Multi-country | Yes (196 countries) | Australia only | USA only | EU-wide |
| Multi-language | Yes (i18n system) | English only | English/Spanish | 5 languages |
| Real-time messaging | Complete (SignalR) | Basic chat | Email only | Email only |
| Mobile app | Not yet (push infra ready) | iOS/Android | Responsive web | iOS/Android |
| Seller verification | Basic (manual) | Government ID linked | Basic | Company registration |

### 5.2 General Agricultural Marketplaces

| Feature | GlobalLivestock | Alibaba Agriculture | IndiaMART | TradeIndia |
|---|---|---|---|---|
| Product categories | 6 types | Broad | Broad | Broad |
| Escrow/Trade Assurance | Missing | Yes (Trade Assurance) | Partial | No |
| RFQ system | Offers exist | Full RFQ | Full RFQ | Full RFQ |
| Verified suppliers | Basic | Gold/Premium tiers | TrustSEAL | Verified badge |
| Price comparison | Missing | Market prices | Yes | Yes |
| Bulk order | MinOrderQuantity field | Full negotiation | Yes | Yes |
| Lead management | Missing | Comprehensive | CRM-like | Basic |

### 5.3 Key Differentiators GlobalLivestock Could Leverage

1. **Multi-country livestock compliance** -- No competitor handles cross-border animal transport documentation comprehensively.
2. **Integrated transport marketplace** -- Having transporters as a first-class role with bidding on transport requests is unique.
3. **Detailed animal data model** -- The AnimalInfo entity is exceptionally detailed (breed, health status, pregnancy, milk production, sire/dam lineage) compared to competitors.
4. **Real-time messaging with context** -- Conversations tied to specific products/orders is superior to email-only competitors.
5. **Veterinary product compliance** -- Tracking withdrawal periods (meat/milk/egg) per medication is regulatory-grade detail.

---

## 6. Multi-Country, Multi-Language, Multi-Currency Assessment

### 6.1 Multi-Country

**Strengths:**
- 196 countries seeded with currency info
- Location entity with ISO country codes
- CountryCode filtering on product search
- Shipping zones per country
- Tax rates per country
- Transporter service regions per country

**Gaps:**
- No country-specific compliance rules (e.g., animal import permits)
- No country-specific payment methods auto-filtering
- User.CountryId and User.LastViewingCountryId exist but no endpoint to update LastViewingCountryId
- No geo-IP based country detection

### 6.2 Multi-Language

**Strengths:**
- Language entity with ISO 639-1 codes
- RTL support flag
- NameTranslations/DescriptionTranslations JSON on Categories and FAQs
- TranslationHelper with fallback chain (exact match -> case-insensitive -> English -> first available -> fallback)

**Gaps:**
- Only Categories and FAQs have translation fields. Missing from: Brands, Banners, PaymentMethods, ShippingCarriers
- Product titles and descriptions are not translated (seller-generated content)
- Error messages are in Turkish (DomainErrors) -- not internationalized
- No automatic translation service integration

### 6.3 Multi-Currency

**Strengths:**
- Currency entity with exchange rates to USD
- ProductPrice entity for multi-currency pricing per product
- Product.Currency field for base currency
- PaymentMethod.SupportedCurrencies
- ShippingRate.Currency

**Gaps:**
- No automatic currency conversion API integration (exchange rates are manual)
- No real-time rate fetching scheduled job
- ProductPrice has IsAutomaticConversion field but no implementation
- Buyer cannot see prices in their preferred currency across all listings
- No currency conversion in search price range filters

---

## 7. Priority Recommendations

### Phase 1: Launch-Blocking (Weeks 1-4)

| # | Feature | Effort | Business Impact |
|---|---|---|---|
| 1 | **Order Management System** (Order, OrderItem entities + full CRUD + status workflow) | High | Critical -- no transactions without this |
| 2 | **Payment Integration** (Stripe Connect for multi-country, Payment entity, escrow flow) | High | Critical -- no revenue without this |
| 3 | **FavoriteProducts/Delete** endpoint | Low | High -- users cannot unfavorite items |
| 4 | **Notifications/MarkAsRead** and **MarkAllAsRead** endpoints | Low | High -- notification badges will accumulate infinitely |
| 5 | **Complete partial CRUD** (AnimalInfos Update/Delete/Detail, HealthRecords Update/Delete/Detail, FeedInfos/ChemicalInfos/MachineryInfos Update/Delete/Detail, ProductPrices Update/Delete/Detail) | Medium | High -- sellers cannot manage their listing details |

### Phase 2: Trust & Safety (Weeks 5-8)

| # | Feature | Effort | Business Impact |
|---|---|---|---|
| 6 | **Dispute Resolution System** (Dispute entity + moderator workflow) | Medium | High -- essential for high-value transactions |
| 7 | **Review Moderation** queue/endpoints | Low | Medium -- prevent fake reviews |
| 8 | **Seller Verification Enhancement** (document upload + multi-step workflow) | Medium | High -- trust signals drive conversion |
| 9 | **User/Content Reporting** system | Low | Medium -- safety requirement |
| 10 | **Automated notification triggers** (offer accepted, deal status change, transport update) | Medium | High -- user engagement |

### Phase 3: Discovery & Growth (Weeks 9-14)

| # | Feature | Effort | Business Impact |
|---|---|---|---|
| 11 | **Advanced Search** (full-text via Elasticsearch, geospatial proximity, multi-faceted animal filters) | High | High -- discovery is core UX |
| 12 | **Auction/Bidding System** | High | High -- major differentiator for livestock |
| 13 | **Seller Public Profile / Storefront** endpoint | Medium | Medium -- seller discoverability |
| 14 | **SearchHistory and ProductViewHistory** endpoints | Low | Medium -- personalization foundation |
| 15 | **Cart System** for consumable products | Medium | Medium -- important for feed/chemical/seed purchases |

### Phase 4: Scale & Differentiate (Weeks 15+)

| # | Feature | Effort | Business Impact |
|---|---|---|---|
| 16 | **Cross-border compliance service** (health certificates, import permits per country) | High | High -- unique differentiator |
| 17 | **Automatic currency conversion** with live exchange rates | Medium | Medium -- better UX for international buyers |
| 18 | **Admin analytics dashboard** (platform GMV, growth metrics) | Medium | Medium -- operational necessity |
| 19 | **Market price index** by breed/region | Medium | High -- value-added content |
| 20 | **GPS transport tracking** mobile integration | High | Medium -- transporter UX |
| 21 | **Product comparison** feature | Low | Low -- nice to have |
| 22 | **Blog/CMS** for agricultural content | Medium | Low -- SEO long-term |

---

## 8. Technical Debt & Data Integrity Concerns

1. **Deal.TotalSales/Revenue on Seller entity:** These are plain counter fields with no backing transaction data. When Order entity is implemented, these should be calculated aggregates.

2. **Product.AverageRating/ReviewCount:** Similarly, these are counter fields that could drift from actual review data. Consider database triggers or event-driven recalculation.

3. **No soft-delete consistency:** BaseEntity has IsDeleted/DeletedAt but some queries may not consistently filter deleted records, especially in navigation properties.

4. **JSON columns risk:** Extensive use of JSON string fields (Attributes, BusinessHours, SocialMediaLinks, FleetInfo, etc.) makes querying and indexing difficult. Consider strongly-typed related entities for critical searchable data.

5. **Missing database indexes:** High-traffic queries (product search by country, category, price range) likely need composite indexes for performance at scale.

---

## Appendix: Complete Endpoint Inventory

### LivestockTrading Module (~130 endpoints)

**Products:** Create, Update, Delete, All, Detail, DetailBySlug, Pick, Search, Approve, Reject, MediaDetail
**Sellers:** Create, Update, Delete, All, Detail, DetailByUserId, Pick, Verify, Suspend
**Transporters:** Create, Update, Delete, All, Detail, Pick, Verify, Suspend
**Categories:** Create, Update, Delete, All, Detail, Pick
**Brands:** Create, Update, Delete, All, Detail, Pick
**Currencies:** Create, Update, Delete, All, Detail, Pick
**Languages:** Create, Update, Delete, All, Detail, Pick
**Locations:** Create, Update, Delete, All, Detail, Pick
**Farms:** Create, Update, Delete, All, Detail, Pick
**Deals:** Create, Update, Delete, All, Detail, Pick
**Offers:** Create, Update, Delete, All, Detail, Pick
**Conversations:** Update, Delete, All, Detail, Pick
**Messages:** Create, Update, Delete, All, Detail, Pick, SendTypingIndicator, UnreadCount
**Notifications:** Create, All, Pick
**FAQs:** Create, Update, Delete, All, Pick
**Banners:** Create, Update, Delete, All, Pick
**PaymentMethods:** Create, Update, Delete, All, Detail, Pick
**ProductPrices:** Create, All, Pick
**ProductVariants:** Create, Update, Delete, All, Detail, Pick
**ProductReviews:** Create, Update, Delete, All, Pick
**SellerReviews:** Create, Update, Delete, All, Detail, Pick
**TransporterReviews:** Create, Update, Delete, All, Detail, Pick
**TransportRequests:** Create, Update, Delete, All, Detail, Pick
**TransportOffers:** Create, Update, Delete, All, Detail, Pick
**TransportTrackings:** Create, Update, Delete, All, Detail, Pick
**ShippingZones:** Create, Update, Delete, All, Detail, Pick
**ShippingRates:** Create, Update, Delete, All, Detail, Pick
**ShippingCarriers:** Create, Update, Delete, All, Detail, Pick
**TaxRates:** Create, Update, Delete, All, Detail, Pick
**FavoriteProducts:** Create, All, Pick
**AnimalInfos:** Create, All, Pick
**FeedInfos:** Create, All, Pick
**ChemicalInfos:** Create, All, Pick
**MachineryInfos:** Create, All, Pick
**SeedInfos:** Create, Update, Delete, All, Detail, Pick
**VeterinaryInfos:** Create, Update, Delete, All, Detail, Pick
**HealthRecords:** Create, All, Pick
**Vaccinations:** Create, Update, Delete, All, Detail, Pick
**UserPreferences:** Create, Update, Delete, All, Detail, Pick
**Dashboard/Stats:** Stats

### IAM Module (~25 endpoints)

**Auth:** Login, Logout, RefreshToken, RevokeRefreshToken, SendOtp, VerifyOtp
**Users:** Create, Update, Delete, Detail, All, ForgotPassword, ResetPassword, UpdatePassword
**Countries:** All
**Provinces:** All
**Districts:** ByProvince
**Neighborhoods:** ByDistrict
**Role:** Create, Update, Delete, All
**Push:** RegisterToken
**UserPermissions:** Query
**MobileApplicationVersion:** GetVersion
