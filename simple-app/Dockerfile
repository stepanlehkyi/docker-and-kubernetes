
FROM golang:1.26.2-alpine AS builder

ARG APP_VERSION=1.0.0.0
ARG ENVIRONMENT=production

WORKDIR /usr/src/app

COPY go.mod go.sum ./
RUN go mod download

COPY . .

RUN go build -o app main.go

FROM alpine:3.21

ARG APP_VERSION=1.0.0.0
ARG ENVIRONMENT=production

RUN adduser -S appuser

WORKDIR /app

COPY --from=builder /usr/src/app/app .

RUN chown appuser /app/app

USER appuser

EXPOSE 8080

ENV APP_VERSION=$APP_VERSION
ENV ENVIRONMENT=$ENVIRONMENT

CMD ["./app"]