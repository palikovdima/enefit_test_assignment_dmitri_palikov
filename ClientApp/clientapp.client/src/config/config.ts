export const BACKEND_URL = import.meta.env.MODE === "development" ? "https://localhost:7190" : "http://localhost:7190";

export const API_VERSION = import.meta.env.VITE_API_VERSION;
export const API_BASE_URL = BACKEND_URL + "/api/" + API_VERSION + "/";

export const PRODUCTS_PAGE_URL = "/" + import.meta.env.VITE_PRODUCTS_PAGE;
export const PRODUCT_HUB_URL = API_BASE_URL + import.meta.env.VITE_PRODUCT_HUB;
export const PRODUCTS_FETCH_URL = API_BASE_URL + import.meta.env.VITE_PRODUCTS_ENDPOINT;

export const CART_PAGE_URL = "/" + import.meta.env.VITE_CART_PAGE;
export const CART_HUB_URL = API_BASE_URL + import.meta.env.VITE_CART_HUB;
export const CART_FETCH_URL = API_BASE_URL + import.meta.env.VITE_CART_ENDPOINT;
export const CART_TOTAL_AMOUNT_URL = CART_FETCH_URL + "/" + import.meta.env.VITE_CART_TOTAL_AMOUNT;
export const CART_CLEAR_URL = CART_FETCH_URL + "/" + import.meta.env.VITE_CART_CLEAR;

export const CHECKOUT_PAGE_URL = "/" + import.meta.env.VITE_CHECKOUT_PAGE;
export const CHECKOUT_URL = API_BASE_URL + import.meta.env.VITE_CHECKOUT_ENDPOINT;
export const CHECKOUT_VALIDATE_PAYMENT_URL = CHECKOUT_URL + "/" + import.meta.env.VITE_CHECKOUT_VALIDATE_PAYMENT;
export const CHECKOUT_CALCULATE_CHANGE_BACK_AMOUNT_URL = CHECKOUT_URL + "/" + import.meta.env.VITE_CHECKOUT_CALCULATE_CHANGE_BACK_AMOUNT;