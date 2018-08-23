
IF NOT EXISTS(
       SELECT 1
       FROM   [dbo].[SpecificationAttribute]
       WHERE  NAME = 'NewProduct'
   )
BEGIN
    DECLARE @specificationAttributeId INT
    SET @specificationAttributeId = -1
    DECLARE @specificationAttributeOprionId INT
    SET @specificationAttributeOprionId = -1
    INSERT INTO [dbo].[SpecificationAttribute]
      (
        [Name],
        [DisplayOrder]
      )
    VALUES
      (
        'NewProduct',
        0
      )
    SET @specificationAttributeId = @@IDENTITY
    IF (@specificationAttributeId > 0)
    BEGIN
        INSERT INTO [dbo].[SpecificationAttributeOption]
          (
            [SpecificationAttributeId],
            [Name],
            [DisplayOrder]
          )
        VALUES
          (
            @specificationAttributeId,
            'True',
            0
          )
        SET @specificationAttributeOprionId = @@IDENTITY
        INSERT INTO [dbo].[SpecificationAttributeOption]
          (
            [SpecificationAttributeId],
            [Name],
            [DisplayOrder]
          )
        VALUES
          (
            @specificationAttributeId,
            'False',
            0
          )
        SELECT @specificationAttributeOprionId
    END
END
ELSE
	SELECT sao.Id
	FROM dbo.SpecificationAttributeOption AS sao
	JOIN dbo.SpecificationAttribute AS sa ON sa.Name = 'NewProduct'
	WHERE sa.Id = sao.SpecificationAttributeId AND sao.Name = 'True'