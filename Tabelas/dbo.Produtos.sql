CREATE TABLE [dbo].[Produtos] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Nome]       NVARCHAR (80) NOT NULL,
    [Item]       NVARCHAR (50) NOT NULL,
    [Valor]      FLOAT (53)    NOT NULL,
    [Quantidade] INT           NOT NULL,
    [Ativo]      BIT           NOT NULL,
    CONSTRAINT [PK_Produtos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

