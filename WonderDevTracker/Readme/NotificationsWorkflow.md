# Notifications Workflow (Ticket Events)

This module creates persisted notifications as a side-effect of ticket lifecycle events
(e.g., assignment, created, resolved, archived).

## Goal

Keep responsibilities separated:

- TicketRepo: "What is the persisted ticket state?"
- RecipientService: "Who should be notified?"
- Templates: "What should the notification say?"
- NotificationOrchestrator: "Coordinate the above and persist notifications."

---

## Workflow (Ticket Assigned)

### 0) Ticket assignment command happens (Ticket domain)
Some Ticket command path (DTO service / command handler) persists the assignment first.

Only AFTER the assignment is saved do we invoke:

`NotificationOrchestrator.TicketAssignedAsync(ticketId, assignedUserId, actorUserInfo)`

---

### 1) NotificationOrchestrator loads persisted truth (TicketRepo)
**Responsibility:** load minimal, tenant-safe ticket snapshot needed for notifications.

- Must verify the ticket exists for the company
- Must reflect persisted state (not caller assumptions)

Example (current implementation uses):
`ticketRepository.GetTicketForNotificationsAsync(ticketId, actor.CompanyId)`

This returns a projection (TicketForNotification) with:
- Ticket Id
- Title
- ProjectId
- SubmitterUserId
- DeveloperUserId

---

### 2) NotificationOrchestrator validates state consistency
**Responsibility:** ensure the event call matches persisted truth.

Example:
- Confirm `ticket.DeveloperUserId == assignedUserId`
- If not, throw (or return) to avoid notifying the wrong people

---

### 3) Recipient resolution (TicketNotificationRecipientService)
**Responsibility:** determine recipients for each role, applying behavioral rules.

This is NOT about authorization.
Authorization happens in the ticket assignment command path.

Recipient service answers: "Given this event, should X receive a notification?"

Common behavioral rules:
- Do not notify the actor about their own action
- Null-safe lookups (PM may not exist)
- (Optional future) preferences: opt-out, digest, etc.

Current methods:
- GetAssignedDeveloperRecipient(...)
- GetProjectManagerRecipientAsync(...) -> queries ProjectRepo for PM id
- GetSubmitterRecipient(...) -> uses SubmitterUserId already present on the ticket projection

---

### 4) Templates (TicketNotificationTemplates)
**Responsibility:** produce consistent titles/messages for each event + recipient role.

Templates should be PURE (no DB, no repositories, no services).
They accept inputs and return strings.

Example signatures:
- (Title, Message) ForTicketAssignedToDeveloper(ticket)
- (Title, Message) ForTicketAssignedToPm(ticket)
- (Title, Message) ForTicketAssignedToSubmitter(ticket)

Why templates matter:
- keeps Orchestrator smaller
- standardizes wording + branding
- allows later localization / formatting rules

---

### 5) NotificationOrchestrator builds Notification entities
**Responsibility:** assemble Notification objects (Title/Message/Type/Ids) and dedupe.

Orchestrator should:
- collect recipients (dev, PM, submitter)
- avoid duplicates (e.g., PM == submitter)
- build Notification entities (Type = Ticket, SenderId = actor, etc.)

---

### 6) Persist (NotificationRepo)
**Responsibility:** write notifications to DB.

Orchestrator persists via:
`INotificationRepository.AddRangeAsync(notifications)`

---

## Responsibility Matrix (Quick Reference)

### TicketRepository
✅ Knows how to read ticket data
✅ Enforces company isolation in queries
✅ Returns lightweight projection for notifications
❌ Does not build notifications
❌ Does not decide recipients

### TicketNotificationRecipientService
✅ Decides "who gets notified"
✅ Applies behavioral rules (skip actor, null safety)
✅ Uses ProjectRepo for PM lookups when needed
❌ Does not query tickets
❌ Does not persist notifications
❌ Does not authorize ticket assignment

### TicketNotificationTemplates
✅ Produces Title/Message consistently
✅ Pure functions (no IO)
❌ No repos/services
❌ No persistence

### NotificationOrchestrator
✅ Coordinates the workflow
✅ Loads ticket snapshot once
✅ Validates event-state consistency
✅ Calls recipient service
✅ Calls templates
✅ Persists notifications
❌ Should not perform authorization checks (those belong upstream in the ticket command)

---

## Suggested Folder Layout

/Services
  /Notifications
    NotificationOrchestrator.cs
    TicketNotificationRecipientService.cs
    TicketNotificationTemplates.cs
/Models
  /Records
    TicketForNotification.cs

---

## Notes / Best Practices

- Call Orchestrator only AFTER SaveChanges on the ticket assignment.
- Avoid extra DB calls:
  - Ticket snapshot query once
  - PM lookup once
- Add deduping in Orchestrator:
  - if PM == dev or PM == submitter, only notify once
- Templates keep wording uniform across the app and reduce orchestrator complexity.
