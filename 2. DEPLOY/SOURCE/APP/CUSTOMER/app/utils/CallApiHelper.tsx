import reactotron from "reactotron-react-native";

export default async function callAPI(
  API?,
  payload?,
  context?,
  onSuccess?,
  onError?,
  onFinaly?,
  typeLoading: "isLoading" | "isDialogLoading" = "isLoading"
) {
  if (context)
    try {
      context.setState({
        [typeLoading]: true
      });
      const res = await API(payload);
      if (onSuccess) onSuccess(res);
      context.setState({
        [typeLoading]: false
      });
    } catch (error) {
      context.setState({
        [typeLoading]: false
      });
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

export async function callAPIHook(
  API?,
  payload?,
  useLoading?,
  onSuccess?,
  onError?,
  onFinaly?
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
