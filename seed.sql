USE TradeHubDb;
SET NOCOUNT ON;

-- Ensure 'alice' exists and capture Id
DECLARE @AliceId bigint = (SELECT TOP 1 Id FROM dbo.Users WHERE Username = 'alice');
IF @AliceId IS NULL
BEGIN
  INSERT INTO dbo.Users (Username, Description, Email, PasswordHash, Role)
  VALUES ('alice', 'demo user', 'alice@example.com', REPLICATE('x', 60), 0);
  SET @AliceId = SCOPE_IDENTITY();
END

-- Ensure 'bob' exists and capture Id
DECLARE @BobId bigint = (SELECT TOP 1 Id FROM dbo.Users WHERE Username = 'bob');
IF @BobId IS NULL
BEGIN
  INSERT INTO dbo.Users (Username, Description, Email, PasswordHash, Role)
  VALUES ('bob', 'demo user', 'bob@example.com', REPLICATE('x', 60), 0);
  SET @BobId = SCOPE_IDENTITY();
END

-- Ensure 'iPhone 10' item for alice
IF NOT EXISTS (SELECT 1 FROM dbo.Items WHERE Name = 'iPhone 10')
BEGIN
  INSERT INTO dbo.Items (Name, Description, Image, Value, OwnerId, Tags, [Condition], Availability, IsValueEstimated)
  VALUES ('iPhone 10', 'Apple iPhone X, 64GB', 'https://example.com/iphone10.png', 300.00, @AliceId,
          'phone,apple,iphone10', 'UsedGood', 'Available', 0);
END

-- Ensure 'Samsung Galaxy S10' item for bob
IF NOT EXISTS (SELECT 1 FROM dbo.Items WHERE Name = 'Samsung Galaxy S10')
BEGIN
  INSERT INTO dbo.Items (Name, Description, Image, Value, OwnerId, Tags, [Condition], Availability, IsValueEstimated)
  VALUES ('Samsung Galaxy S10', 'Galaxy S10, 128GB', 'https://example.com/galaxy-s10.png', 280.00, @BobId,
          'phone,samsung,galaxy', 'UsedGood', 'Available', 0);
END

-- Create a Pending trade alice -> bob if none exists
DECLARE @TradeId bigint = (SELECT TOP 1 Id FROM dbo.Trades WHERE InitiatedId=@AliceId AND ReceivedId=@BobId AND Status=0 ORDER BY Id DESC);
IF @TradeId IS NULL
BEGIN
  INSERT INTO dbo.Trades (CreatedAt, InitiatedId, ReceivedId, Status)
  VALUES (SYSUTCDATETIME(), @AliceId, @BobId, 0);
  SET @TradeId = SCOPE_IDENTITY();
END

-- Attach both items to the trade (idempotent)
UPDATE i SET i.TradeId = @TradeId
FROM dbo.Items i
WHERE i.Name IN ('iPhone 10','Samsung Galaxy S10') AND (i.TradeId IS NULL OR i.TradeId <> @TradeId);

-- Show a quick summary
SELECT TOP 5 Id, InitiatedId, ReceivedId, Status FROM dbo.Trades ORDER BY Id DESC;
SELECT TOP 5 Id, Name, OwnerId, TradeId FROM dbo.Items WHERE Name IN ('iPhone 10','Samsung Galaxy S10') ORDER BY Id DESC;
