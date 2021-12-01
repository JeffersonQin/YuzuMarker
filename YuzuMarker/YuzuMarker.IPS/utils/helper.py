import traceback


def get_server_exception_message(exception):
    """
    Returns the exception message
    :param exception: Exception
    :return: str
    """
    return {
        'status': 'failed',
        'error': f'{repr(exception)}\n{traceback.format_exc()}',
        'data': {}
    }


def get_server_success_message(data):
    """
    Returns the success message
    :param data: Any
    :return: dict
    """
    return {
        'status': 'success',
        'error': '',
        'data': data
    }
