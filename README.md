# ⚡ Energy Management System
## Take control of your energy usage with intelligence and automation.

## The Energy Management System (EMS) is a comprehensive web platform that enables clients and administrators to efficiently monitor, manage, and analyze energy consumption. The system integrates smart metering devices, real-time monitoring, and a microservices architecture to provide accurate energy insights and intelligent alerts.

## 🔍 Overview
This application is built to support two user roles:

## 🛠️ Administrators

Full CRUD (Create, Read, Update, Delete) capabilities for users and smart energy metering devices

Manage associations between users and devices

## 👤 Clients

View devices associated with their accounts

Monitor energy usage through a clean, user-friendly interface

## 🧠 Microservices Architecture
## 📡 Monitoring & Communication Microservice
Collects data from smart metering devices

Computes hourly energy consumption

Stores processed data in a dedicated database

Syncs data with the Device Management Microservice using an event-driven approach (RabbitMQ)

## 🔁 Device Management Microservice
Manages users and devices

Exposes endpoints for CRUD operations

Sends device events to the Monitoring Microservice via RabbitMQ

## 🧪 Smart Metering Device Simulator
Reads energy data from sensor.csv

Sends JSON-formatted energy readings to the message broker

Simulates real-time energy usage from hardware sensors

## ⚠️ Real-Time Alerts
When consumption exceeds defined thresholds, users are notified instantly via WebSocket (SignalR) updates in the web UI
