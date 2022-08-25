export default async function callAPI(
  API,
  payload,
  context,
  onSuccess,
  onError
) {
  try {
    if (context)
      context.setState({
        ...context.state,
        isLoading: true
      });
    const res = await API(payload);
    if (onSuccess) onSuccess(res);
    if (context)
      context.setState({
        ...context.state,
        isLoading: false
      });
    return res;
  } catch (error) {
    if (context)
      context.setState({
        ...context.state,
        isLoading: false
      });
    if (onError) onError(error);
  }
}
export async function callAPIHook(
  API,
  payload,
  useLoading,
  onSuccess,
  onError,
  onFinaly
) {
  if (useLoading)
    try {
      useLoading(true);
      const res = await API(payload);
      if (onSuccess) onSuccess(res);
      useLoading(false);
    } catch (error) {
      useLoading(false);
      if (onError) onError(error);
    } finally {
      if (onFinaly) onFinaly();
    }
  else
    try {
      const res = await API(payload);

      if (onSuccess) onSuccess(res);
    } catch (error) {
      if (onError) onError(error);
    } finally {
      if (onFinaly) onFinaly();
    }
}
