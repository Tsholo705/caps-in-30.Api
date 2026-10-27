IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913071655_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Username] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Grade] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913071655_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913071655_InitialCreate', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913111627_UpdateApplicationModel'
)
BEGIN
    CREATE TABLE [Subjects] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913111627_UpdateApplicationModel'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [SubjectId] int NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Categories_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913111627_UpdateApplicationModel'
)
BEGIN
    CREATE TABLE [Questions] (
        [Id] int NOT NULL IDENTITY,
        [Grade] int NOT NULL,
        [CategoryId] int NOT NULL,
        [QuestionText] nvarchar(max) NOT NULL,
        [Options] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Questions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913111627_UpdateApplicationModel'
)
BEGIN
    CREATE INDEX [IX_Categories_SubjectId] ON [Categories] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913111627_UpdateApplicationModel'
)
BEGIN
    CREATE INDEX [IX_Questions_CategoryId] ON [Questions] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913111627_UpdateApplicationModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913111627_UpdateApplicationModel', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Categories] DROP CONSTRAINT [FK_Categories_Subjects_SubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Questions] DROP CONSTRAINT [FK_Questions_Categories_CategoryId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Questions] ADD [CorrectIndex] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Questions] ADD [SubjectId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    CREATE INDEX [IX_Questions_SubjectId] ON [Questions] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Categories] ADD CONSTRAINT [FK_Categories_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Questions] ADD CONSTRAINT [FK_Questions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    ALTER TABLE [Questions] ADD CONSTRAINT [FK_Questions_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913123316_AddQuestionAnswerAndRelationships'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913123316_AddQuestionAnswerAndRelationships', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922002558_AddScores'
)
BEGIN
    CREATE TABLE [Scores] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Subject] nvarchar(max) NOT NULL,
        [Topic] nvarchar(max) NOT NULL,
        [Grade] int NOT NULL,
        [Correct] int NOT NULL,
        [Total] int NOT NULL,
        [Percentage] int NOT NULL,
        [TimeUsed] int NOT NULL,
        [Date] datetime2 NOT NULL,
        CONSTRAINT [PK_Scores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Scores_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922002558_AddScores'
)
BEGIN
    CREATE INDEX [IX_Scores_UserId] ON [Scores] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922002558_AddScores'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922002558_AddScores', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922022723_AddTeamGames'
)
BEGIN
    CREATE TABLE [TeamGames] (
        [Id] int NOT NULL IDENTITY,
        [Grade] int NOT NULL,
        [Subject] nvarchar(max) NOT NULL,
        [Topic] nvarchar(max) NOT NULL,
        [NumberOfTeams] int NOT NULL,
        [NumberOfRounds] int NOT NULL,
        [CurrentTeam] int NOT NULL,
        [CurrentRound] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IsFinished] bit NOT NULL,
        CONSTRAINT [PK_TeamGames] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922022723_AddTeamGames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922022723_AddTeamGames', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922023402_AddTeamCards'
)
BEGIN
    CREATE TABLE [TeamCards] (
        [Id] int NOT NULL IDENTITY,
        [Grade] int NOT NULL,
        [SubjectId] int NOT NULL,
        [CategoryId] int NOT NULL,
        [Term] nvarchar(max) NOT NULL,
        [ForbiddenWords] nvarchar(max) NOT NULL,
        [Points] int NOT NULL,
        CONSTRAINT [PK_TeamCards] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TeamCards_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TeamCards_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922023402_AddTeamCards'
)
BEGIN
    CREATE INDEX [IX_TeamCards_CategoryId] ON [TeamCards] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922023402_AddTeamCards'
)
BEGIN
    CREATE INDEX [IX_TeamCards_SubjectId] ON [TeamCards] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260922023402_AddTeamCards'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260922023402_AddTeamCards', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006100701_AddEmailVerification'
)
BEGIN
    ALTER TABLE [Users] ADD [EmailVerificationToken] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006100701_AddEmailVerification'
)
BEGIN
    ALTER TABLE [Users] ADD [EmailVerificationTokenExpiry] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006100701_AddEmailVerification'
)
BEGIN
    ALTER TABLE [Users] ADD [IsEmailVerified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006100701_AddEmailVerification'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006100701_AddEmailVerification', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006160326_AddAccessCodes'
)
BEGIN
    CREATE TABLE [AccessCodes] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(max) NOT NULL,
        [Grade] int NOT NULL,
        [IsUsed] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UsedAt] datetime2 NULL,
        CONSTRAINT [PK_AccessCodes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006160326_AddAccessCodes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006160326_AddAccessCodes', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012152804_RemoveEmailVerification'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Email');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Users] DROP COLUMN [Email];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012152804_RemoveEmailVerification'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'EmailVerificationToken');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Users] DROP COLUMN [EmailVerificationToken];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012152804_RemoveEmailVerification'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'EmailVerificationTokenExpiry');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Users] DROP COLUMN [EmailVerificationTokenExpiry];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012152804_RemoveEmailVerification'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'IsEmailVerified');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Users] DROP COLUMN [IsEmailVerified];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012152804_RemoveEmailVerification'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261012152804_RemoveEmailVerification', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261023162824_AddSpellingWords'
)
BEGIN
    CREATE TABLE [SpellingWords] (
        [Id] int NOT NULL IDENTITY,
        [Grade] int NOT NULL,
        [Word] nvarchar(max) NOT NULL,
        [SubjectId] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_SpellingWords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SpellingWords_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SpellingWords_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261023162824_AddSpellingWords'
)
BEGIN
    CREATE INDEX [IX_SpellingWords_CategoryId] ON [SpellingWords] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261023162824_AddSpellingWords'
)
BEGIN
    CREATE INDEX [IX_SpellingWords_SubjectId] ON [SpellingWords] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261023162824_AddSpellingWords'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261023162824_AddSpellingWords', N'10.0.12');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026170103_AddTimetablesAndStudySessions'
)
BEGIN
    CREATE TABLE [Timetables] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Timetables] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Timetables_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026170103_AddTimetablesAndStudySessions'
)
BEGIN
    CREATE TABLE [StudySessions] (
        [Id] int NOT NULL IDENTITY,
        [TimetableId] int NOT NULL,
        [Date] nvarchar(max) NOT NULL,
        [StartTime] nvarchar(max) NOT NULL,
        [Minutes] int NOT NULL,
        [Subject] nvarchar(max) NOT NULL,
        [Topic] nvarchar(max) NOT NULL,
        [Notes] nvarchar(max) NOT NULL,
        [IsComplete] bit NOT NULL,
        CONSTRAINT [PK_StudySessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StudySessions_Timetables_TimetableId] FOREIGN KEY ([TimetableId]) REFERENCES [Timetables] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026170103_AddTimetablesAndStudySessions'
)
BEGIN
    CREATE INDEX [IX_StudySessions_TimetableId] ON [StudySessions] ([TimetableId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026170103_AddTimetablesAndStudySessions'
)
BEGIN
    CREATE INDEX [IX_Timetables_UserId] ON [Timetables] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026170103_AddTimetablesAndStudySessions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261026170103_AddTimetablesAndStudySessions', N'10.0.12');
END;

COMMIT;
GO

