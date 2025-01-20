FROM node:20.10.0-alpine as client-builder
RUN mkdir -p /app
WORKDIR /app
COPY package.json /app
RUN npm install
COPY . /app

RUN npm run build --prod

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=client-builder /app/dist/client /usr/share/nginx/html
EXPOSE 4200
