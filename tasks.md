# PickC API Migration — Phase Tracker

## Phase 1: Foundation (SharedKernel + Infrastructure + Identity + API Host)
- [x] SharedKernel — BaseEntity, AuditableEntity, IRepository, Exceptions, Helpers, Extensions
- [x] Shared Infrastructure — Middleware (Exception, Logging, CorrelationId), Persistence, Extensions
- [x] Identity Module — JWT, AuthService, OtpService, IdentityDbContext, AuthController, OtpController
- [x] API Host — Program.cs, appsettings, Swagger, JWT config, Module registration
- [x] Build verification

## Phase 2: Master Module
- [x] Domain entities from actual DB schema (14 entities)
- [x] EF Core configurations and MasterDbContext (14 configurations)
- [x] Repository interfaces and implementations (Customer, Driver, Address, LookUp)
- [x] DTOs, Application Services, FluentValidation validators
- [x] API Controllers (Customer, Driver, Address, LookUp, RateCard, VehicleConfig, Manufacturer, Vehicle)
- [x] MasterModule registration + build verification

## Phase 3: Booking Module
- [x] Domain entities (Booking, DriverCancellationHistory) from actual DB schema (36 + 4 columns)
- [x] BookingDbContext + EF Core configurations (2 configurations, Operation schema)
- [x] Booking repository (CRUD, confirm, cancel by customer/driver, complete, reach pickup/destination, search, near bookings)
- [x] DTOs (BookingDto, BookingSaveDto, BookingConfirmDto, BookingCancelDto, BookingDriverCancelDto, BookingSearchDto, NearBookingDto)
- [x] BookingService + IBookingService, FluentValidation validators (Save, Cancel, DriverCancel, Confirm)
- [x] API Controllers (BookingController — 12 endpoints, SearchController — 2 endpoints)
- [x] BookingModule registration + build verification (0 warnings, 0 errors)

## Phase 4: Trip Module
- [x] Domain entities (Trip — 23 cols, TripMonitor — 7 cols) from actual DB schema
- [x] TripDbContext + EF Core configurations (2 configurations, Operation schema)
- [x] Trip repositories (ITripRepository + TripRepository: CRUD, end trip, update distance, current by driver/customer; ITripMonitorRepository + TripMonitorRepository: save, get by trip/driver)
- [x] DTOs (TripDto, TripSaveDto, TripEndDto, TripUpdateDistanceDto, TripMonitorDto, TripMonitorSaveDto), Services (TripService, TripMonitorService), Validators (TripSave, TripEnd, TripMonitorSave)
- [x] API Controllers (TripController — 10 endpoints, TripMonitorController — 4 endpoints)
- [x] TripModule registration + build verification (0 warnings, 0 errors)

## Phase 5: Billing Module
- [x] Domain entity (Invoice — 13 cols, composite PK InvoiceNo+TripID) from actual DB schema. DriverPayment/DriverSummary are query DTOs (no tables).
- [x] BillingDbContext + InvoiceConfiguration (Operation schema)
- [x] IInvoiceRepository + InvoiceRepository (CRUD, mark paid, mark mail sent)
- [x] DTOs (InvoiceDto, InvoiceSaveDto, InvoicePayDto, InvoiceMailDto, DriverPaymentDto, DriverSummaryDto)
- [x] InvoiceService + DriverBillingService (raw SQL for cross-module aggregation replacing usp_GetDriverPayments and usp_GetDriverSummary)
- [x] Validators (InvoiceSaveValidator, InvoicePayValidator)
- [x] API Controllers (InvoiceController — 8 endpoints, PaymentController — 2 endpoints)
- [x] BillingModule registration + build verification (0 warnings, 0 errors)

## Phase 6: Notification Module
- [x] IPushNotificationService + FcmPushNotificationService (Firebase Admin SDK, singleton, graceful degradation if credentials missing)
- [x] ISmsService + SmsService (typed HttpClient, SmsSettings from config, URI-escaped parameters)
- [x] OTP delivery wired via ISmsService (available to Identity module's OtpService)
- [x] NotificationModule registration + build verification (0 warnings, 0 errors)

## Phase 7: Integration Testing & Hardening
- [x] Test infrastructure (PickCApiFactory with InMemory DB replacing all 5 DbContexts, DevBypass auth)
- [x] HealthCheck tests (2 tests: /health, /swagger/v1/swagger.json)
- [x] Master module tests (4 tests: GetAll, Save+Get, Delete, GetAllLookUps)
- [x] Booking module tests (4 tests: GetAll, Save+GetById, Save+GetByCustomer, SearchToday, NearBookings)
- [x] Trip module tests (5 tests: GetAll, Save+GetById, CurrentByDriver-NotFound, SaveMonitor, GetAllMonitors)
- [x] Billing module tests (4 tests: GetAll, Save+GetByKey, Save+GetByBookingNo, GetByBookingNo-NotFound)
- [x] All 20 integration tests passing (ExecuteUpdateAsync operations skipped — InMemory limitation)
