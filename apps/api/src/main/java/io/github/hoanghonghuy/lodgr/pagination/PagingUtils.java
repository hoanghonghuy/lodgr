package io.github.hoanghonghuy.lodgr.pagination;

public final class PagingUtils {
    public static final int DEFAULT_PAGE_NUMBER = 1;
    public static final int DEFAULT_PAGE_SIZE = 20;
    public static final int MAX_PAGE_SIZE = 100;

    private PagingUtils() {
    }

    public static int normalizePageNumber(Integer pageNumber) {
        if (pageNumber == null || pageNumber < 1) {
            return DEFAULT_PAGE_NUMBER;
        }

        return pageNumber;
    }

    public static int normalizePageSize(Integer pageSize) {
        if (pageSize == null || pageSize < 1) {
            return DEFAULT_PAGE_SIZE;
        }

        return Math.min(pageSize, MAX_PAGE_SIZE);
    }
}
