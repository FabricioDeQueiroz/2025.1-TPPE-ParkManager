const getFirstValidationError = (errorsObj) => {
    if (!errorsObj) return '';
    const firstKey = Object.keys(errorsObj)[0];
    return errorsObj[firstKey][0];
};

export default getFirstValidationError;
