using Irrelephant.Search.PrivateEye.Tests.Integration.SearchInfrastructure;

namespace Irrelephant.Search.PrivateEye.Tests.Integration.Fixtures;

public class TestData
{
    public static readonly SampleDocument[] Documents = {
        new()
        {
            Id = Guid.NewGuid().ToString("D"),
            SomeNumber = 42,
            SomeText = "Every good sentence must include the word 'Bacon'."
        },
        new()
        {
            Id = Guid.NewGuid().ToString("D"),
            SomeNumber = 13,
            SomeText = "A cat is eating the lettuce. He seems to be enjoying it too!"
        },
        new()
        {
            Id = Guid.NewGuid().ToString("D"),
            SomeNumber = 37,
            SomeText = "I have purchased thirty seven rolls of tape."
        }
    };
}
