export const DateConverter = (dateString, isGmt, anoSlice) => {
    const sum = isGmt ? 3 : 0;

    const date = new Date(dateString);

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = String(date.getFullYear()).slice(-anoSlice);

    const hours = String(date.getHours() + sum).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${day}/${month}/${year} ${hours}:${minutes}`;
};
