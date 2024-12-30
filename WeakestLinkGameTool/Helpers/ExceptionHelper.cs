namespace WeakestLinkGameTool.Helpers;

public static class ExceptionHelper {

    public static void ThrowOnFail(bool condition) {
        if (!condition) throw new Exception("Condition failed");
    }
}