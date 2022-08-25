export function checkPhoneNumber(phone_number) {
  return /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/.test(
    phone_number
  );
}
