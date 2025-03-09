FROM node:18 AS build
WORKDIR /app

COPY ClientApp/clientapp.client/package.json ClientApp/clientapp.client/package-lock.json ./

RUN npm install

COPY ClientApp/clientapp.client ./

RUN npm run build

FROM nginx:latest AS production
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
