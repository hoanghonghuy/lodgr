package org.lodgr.api.pagination;

import java.util.List;

public final class PagedResult<T> {
    private final List<T> items;
    private final int pageNumber;
    private final int pageSize;
    private final long totalItems;
    private final int totalPages;
    private final boolean hasNextPage;
    private final boolean hasPreviousPage;

    public PagedResult(
            List<T> items,
            int pageNumber,
            int pageSize,
            long totalItems,
            int totalPages,
            boolean hasNextPage,
            boolean hasPreviousPage) {
        this.items = items;
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.totalItems = totalItems;
        this.totalPages = totalPages;
        this.hasNextPage = hasNextPage;
        this.hasPreviousPage = hasPreviousPage;
    }

    public static <T> PagedResult<T> create(List<T> items, int pageNumber, int pageSize, long totalItems) {
        int normalizedPageSize = pageSize <= 0 ? 1 : pageSize;
        int totalPages = (int) Math.ceil(totalItems / (double) normalizedPageSize);

        return new PagedResult<>(
                items,
                pageNumber,
                pageSize,
                totalItems,
                totalPages,
                pageNumber < totalPages,
                pageNumber > 1);
    }

    public List<T> getItems() {
        return items;
    }

    public int getPageNumber() {
        return pageNumber;
    }

    public int getPageSize() {
        return pageSize;
    }

    public long getTotalItems() {
        return totalItems;
    }

    public int getTotalPages() {
        return totalPages;
    }

    public boolean isHasNextPage() {
        return hasNextPage;
    }

    public boolean isHasPreviousPage() {
        return hasPreviousPage;
    }
}
