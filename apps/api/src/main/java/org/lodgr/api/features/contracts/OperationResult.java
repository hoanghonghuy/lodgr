package org.lodgr.api.features.contracts;

public final class OperationResult<T> {
    public static final class OperationError {
        private final OperationErrorType type;
        private final String message;

        public OperationError(OperationErrorType type, String message) {
            this.type = type;
            this.message = message;
        }

        public OperationErrorType getType() {
            return type;
        }

        public String getMessage() {
            return message;
        }
    }

    private final boolean success;
    private final T value;
    private final OperationError error;

    private OperationResult(boolean success, T value, OperationError error) {
        this.success = success;
        this.value = value;
        this.error = error;
    }

    public static <T> OperationResult<T> success(T value) {
        return new OperationResult<>(true, value, null);
    }

    public static <T> OperationResult<T> failure(OperationErrorType type, String message) {
        return new OperationResult<>(false, null, new OperationError(type, message));
    }

    public boolean isSuccess() {
        return success;
    }

    public T getValue() {
        return value;
    }

    public OperationError getError() {
        return error;
    }
}
