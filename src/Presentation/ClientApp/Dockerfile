FROM nginx:alpine
LABEL author="jcurbelo"
COPY ./dist /usr/share/nginx/html

# Custom nginx config
COPY ./nginx-custom.conf /etc/nginx/conf.d/default.conf

EXPOSE 80 443
ENTRYPOINT [ "nginx", "-g", "daemon off;" ]
