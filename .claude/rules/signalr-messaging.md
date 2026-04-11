---
paths:
  - "**/Hubs/**"
  - "**/Messages/**"
  - "**/Conversations/**"
  - "**/NotificationSender/**"
  - "**/Events/**"
  - "**/PresenceService*"
---
# Real-Time Messaging (SignalR)

Platform, kullanicilar arasi gercek zamanli mesajlasma icin SignalR kullanir.

## SignalR Hub

**Dosya:** `LivestockTrading.Api/Hubs/ChatHub.cs`

```csharp
// Hub endpoint: /hubs/chat
// JWT Authentication gerekli

// Client -> Server Methods:
Task JoinConversation(Guid conversationId)      // Conversation'a katil
Task LeaveConversation(Guid conversationId)     // Conversation'dan ayril
Task SendTypingIndicator(Guid conversationId, bool isTyping)  // Yaziyor gostergesi
Task MarkMessageAsRead(Guid messageId)          // Mesaji okundu isaretla
Task<List<Guid>> GetOnlineUsers(List<Guid> userIds)  // Online kullanicilari sorgula

// Server -> Client Events:
ReceiveMessage(message)        // Yeni mesaj geldi
TypingIndicator(indicator)     // Yaziyor gostergesi
MessageRead(data)              // Mesaj okundu
UserOnline(userId)             // Kullanici cevrimici
UserOffline(userId)            // Kullanici cevrimdisi
```

## Domain Events

**Dosya:** `LivestockTrading.Domain/Events/MessagingEvents.cs`

| Event | Tetikleyen | Aciklama |
|-------|------------|----------|
| `MessageCreatedEvent` | Messages/Create/Handler | Yeni mesaj gonderildi |
| `MessageReadEvent` | Messages/Update/Handler | Mesaj okundu |
| `ConversationCreatedEvent` | Conversations/Create/Handler | Yeni konusma baslatildi |
| `TypingIndicatorEvent` | Messages/SendTypingIndicator/Handler | Yaziyor gostergesi |

## Event Publishing Pattern

Handler'larda RabbitMQ ile event publish:

```csharp
// Handler.cs
private readonly IRabbitMqPublisher _publisher;
private readonly CurrentUserService _currentUserService;

public async Task<ArfBlocksRequestResult> Handle(...)
{
    // ... entity olustur/guncelle ...

    // Event publish (NotificationSender Worker dinler)
    await _publisher.PublishFanout("livestocktrading.notification.push", new MessageCreatedEvent
    {
        MessageId = entity.Id,
        ConversationId = entity.ConversationId,
        SenderUserId = entity.SenderUserId,
        RecipientUserId = entity.RecipientUserId,
        SenderName = _currentUserService.GetCurrentUserDisplayName(),  // ONEMLI
        Content = entity.Content,
        CreatedAt = entity.SentAt
    });
}
```

## Notification Worker Event Handlers

**Konum:** `Workers/NotificationSender/EventHandlers/`

| Handler | Event | Islem |
|---------|-------|-------|
| `MessageCreatedNotificationHandler` | MessageCreatedEvent | Push notification gonder |
| `MessageReadNotificationHandler` | MessageReadEvent | SignalR ile bildir |
| `ConversationCreatedNotificationHandler` | ConversationCreatedEvent | Push notification gonder |

## Presence Service

**Dosya:** `LivestockTrading.Application/Services/PresenceService.cs`

Redis ile online/offline durum takibi:

```csharp
// Kullaniciyi online yap
await _presenceService.SetUserOnlineAsync(userId, connectionId);

// Kullaniciyi offline yap
await _presenceService.SetUserOfflineAsync(userId, connectionId);

// Online durumu kontrol et
bool isOnline = await _presenceService.IsUserOnlineAsync(userId);
```

## Messaging Endpoint'leri

| Endpoint | Aciklama |
|----------|----------|
| `POST /Conversations/Create` | Yeni konusma baslat |
| `POST /Conversations/All` | Konusma listesi |
| `POST /Conversations/Detail` | Konusma detayi |
| `POST /Messages/Create` | Mesaj gonder |
| `POST /Messages/All` | Mesaj listesi |
| `POST /Messages/Update` | Mesaji guncelle (okundu) |
| `POST /Messages/SendTypingIndicator` | Yaziyor gostergesi |

## Ilgili Dosyalar

```
LivestockTrading.Api/
├── Hubs/ChatHub.cs                          # SignalR Hub
└── Program.cs                               # SignalR DI & MapHub

LivestockTrading.Domain/
└── Events/MessagingEvents.cs                # Domain events

LivestockTrading.Application/
├── Services/PresenceService.cs              # Online/offline tracking
└── RequestHandlers/
    ├── Conversations/Commands/Create/       # Konusma olustur
    └── Messages/
        ├── Commands/Create/                 # Mesaj gonder
        ├── Commands/Update/                 # Mesaj guncelle
        └── Commands/SendTypingIndicator/    # Yaziyor gostergesi

Workers/NotificationSender/
├── EventHandlers/
│   ├── MessageCreatedNotificationHandler.cs
│   ├── MessageReadNotificationHandler.cs
│   └── ConversationCreatedNotificationHandler.cs
└── Workers/NotificationWorker.cs            # Event consumer

Gateways/Api/ocelot.json                     # WebSocket route
```
