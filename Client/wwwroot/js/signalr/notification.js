"use strict";

// Create connection to the notification hub
const notificationConnection = new signalR.HubConnectionBuilder()
  .withUrl("/notificationHub")
  .withAutomaticReconnect()
  .build();

// Start the connection
function startNotificationConnection() {
  notificationConnection
    .start()
    .then(() => {
      console.log("Notification hub connection established");
      // You can join groups or do other initialization here
    })
    .catch((err) => {
      console.error("Error while establishing notification connection: " + err);
      // Retry connection after 5 seconds
      setTimeout(startNotificationConnection, 5000);
    });
}

// Handle connection closed
notificationConnection.onclose((error) => {
  console.log("Notification hub connection closed");
  if (error) {
    console.error("Connection closed due to error: " + error);
  }
  // Attempt to restart the connection
  setTimeout(startNotificationConnection, 5000);
});

// Register handlers for notification events
notificationConnection.on("ReceiveNotification", (user, message) => {
  // Display the notification to the user
  showNotification(user, message);
});

notificationConnection.on("ReceivePrivateNotification", (message) => {
  // Display private notification
  showPrivateNotification(message);
});

notificationConnection.on("ReceiveGroupNotification", (message) => {
  // Display group notification
  showGroupNotification(message);
});

// Helper functions to display notifications
function showNotification(user, message) {
  // Create notification element
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification";
  notificationElement.innerHTML = `<strong>${user}:</strong> ${message}`;

  // Add notification to container
  const notificationContainer = document.getElementById(
    "notification-container"
  );
  if (notificationContainer) {
    notificationContainer.appendChild(notificationElement);

    // Auto remove after 5 seconds
    setTimeout(() => {
      notificationElement.classList.add("fade-out");
      setTimeout(() => {
        notificationContainer.removeChild(notificationElement);
      }, 500);
    }, 5000);
  }
}

function showPrivateNotification(message) {
  // Create private notification with different styling
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification private-notification";
  notificationElement.textContent = message;

  // Add notification to container
  const notificationContainer = document.getElementById(
    "notification-container"
  );
  if (notificationContainer) {
    notificationContainer.appendChild(notificationElement);

    // Auto remove after 5 seconds
    setTimeout(() => {
      notificationElement.classList.add("fade-out");
      setTimeout(() => {
        notificationContainer.removeChild(notificationElement);
      }, 500);
    }, 5000);
  }
}

function showGroupNotification(message) {
  // Create group notification with different styling
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification group-notification";
  notificationElement.textContent = message;

  // Add notification to container
  const notificationContainer = document.getElementById(
    "notification-container"
  );
  if (notificationContainer) {
    notificationContainer.appendChild(notificationElement);

    // Auto remove after 5 seconds
    setTimeout(() => {
      notificationElement.classList.add("fade-out");
      setTimeout(() => {
        notificationContainer.removeChild(notificationElement);
      }, 500);
    }, 5000);
  }
}

// Start the connection when the document is ready
document.addEventListener("DOMContentLoaded", () => {
  startNotificationConnection();
});
