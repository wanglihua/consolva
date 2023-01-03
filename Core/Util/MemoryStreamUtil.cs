using Microsoft.IO;

namespace shunda.Core.Util;

public class MemoryStreamUtil {
    private const int BLOCK_SIZE = 1024;
    private const int LARGE_BUFFER_MULTIPLE = 1024 * 1024;
    private const int MAX_BUFFER_SIZE = 16 * LARGE_BUFFER_MULTIPLE;

    private static RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager(BLOCK_SIZE, LARGE_BUFFER_MULTIPLE, MAX_BUFFER_SIZE) {
        // GenerateCallStacks = true,
        // AggressiveBufferReturn = true,
        MaximumFreeLargePoolBytes = MAX_BUFFER_SIZE * 4,
        MaximumFreeSmallPoolBytes = 100 * BLOCK_SIZE,
    };

    public static MemoryStream GetStream() {
        return manager.GetStream();
    }
}