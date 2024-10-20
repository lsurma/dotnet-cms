namespace Surma.CMS.Dev.CMS;

public class TestEntity
{
    public TestEntityId Id { get; set; }
    
    public required string Name { get; set; }
}

public readonly record struct TestEntityId(Guid Value);
