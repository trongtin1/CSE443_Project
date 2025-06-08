"use strict";

// Create connection to the job hub
const jobConnection = new signalR.HubConnectionBuilder()
  .withUrl("/jobHub")
  .withAutomaticReconnect()
  .build();

// Start the connection
function startJobConnection() {
  jobConnection
    .start()
    .then(() => {
      console.log("Job hub connection established");

      // Join job categories that the user is interested in
      // This should be populated based on user preferences
      const userCategories = getUserPreferredCategories();
      if (userCategories && userCategories.length > 0) {
        userCategories.forEach((category) => {
          jobConnection
            .invoke("JoinJobCategory", category)
            .catch((err) =>
              console.error(`Error joining category ${category}: ${err}`)
            );
        });
      }

      // Join as JobSeeker if applicable
      const jobSeekerId = getJobSeekerId();
      if (jobSeekerId) {
        console.log(`Joining as JobSeeker: ${jobSeekerId}`);
        jobConnection
          .invoke("JoinAsJobSeeker", jobSeekerId)
          .then(() =>
            console.log(`Successfully joined as JobSeeker ${jobSeekerId}`)
          )
          .catch((err) => console.error(`Error joining as JobSeeker: ${err}`));
      }
    })
    .catch((err) => {
      console.error("Error while establishing job connection: " + err);
      // Retry connection after 5 seconds
      setTimeout(startJobConnection, 5000);
    });
}

// Handle connection closed
jobConnection.onclose((error) => {
  console.log("Job hub connection closed");
  if (error) {
    console.error("Connection closed due to error: " + error);
  }
  // Attempt to restart the connection
  setTimeout(startJobConnection, 5000);
});

// Helper function to get user's preferred job categories
function getUserPreferredCategories() {
  // This should be implemented to get categories from user preferences
  // For now, return an empty array or hardcoded values for testing
  return [];
}

// Helper function to get JobSeeker ID from session
function getJobSeekerId() {
  // Try to get JobSeeker ID from page data
  const jobSeekerIdElement = document.getElementById("current-jobseeker-id");
  if (jobSeekerIdElement) {
    return jobSeekerIdElement.value;
  }

  // Try to get from localStorage if available
  if (localStorage.getItem("jobSeekerId")) {
    return localStorage.getItem("jobSeekerId");
  }

  return null;
}

// Register handlers for job-related events
jobConnection.on("ReceiveNewJob", (jobId, jobTitle, company) => {
  showNewJobNotification(jobId, jobTitle, company);
});

jobConnection.on("ApplicationStatusChanged", (applicationId, status) => {
  console.log(
    `Received application status change: Application #${applicationId} changed to ${status}`
  );
  showApplicationStatusNotification(applicationId, status);
});

jobConnection.on("NewApplication", (jobId, applicantName) => {
  showNewApplicationNotification(jobId, applicantName);
});

jobConnection.on("NewJobInCategory", (category, jobId, jobTitle) => {
  showNewJobInCategoryNotification(category, jobId, jobTitle);
});

// Helper functions to display job-related notifications
function showNewJobNotification(jobId, jobTitle, company) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification job-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>New Job Posted:</strong> ${jobTitle} at ${company}
            <a href="/Job/Details/${jobId}" class="notification-link">View Job</a>
        </div>
    `;

  showNotificationElement(notificationElement);
}

function showApplicationStatusNotification(applicationId, status) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification application-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>Application Update:</strong> Your application status changed to "${status}"
            <a href="/Application/Details/${applicationId}" class="notification-link">View Application</a>
        </div>
    `;

  showNotificationElement(notificationElement);
}

function showNewApplicationNotification(jobId, applicantName) {
  // Tạo thông báo dạng popup
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification employer-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>Đơn ứng tuyển mới:</strong> ${applicantName} đã ứng tuyển vào công việc của bạn
            <a href="/Employer/Applications" class="notification-link">Xem đơn ứng tuyển</a>
        </div>
    `;

  // Hiển thị popup thông báo với phần tử HTML
  showNotificationElement(notificationElement);
}

function showNewJobInCategoryNotification(category, jobId, jobTitle) {
  const notificationElement = document.createElement("div");
  notificationElement.className = "notification category-notification";
  notificationElement.innerHTML = `
        <div class="notification-content">
            <strong>New ${category} Job:</strong> ${jobTitle}
            <a href="/Job/Details/${jobId}" class="notification-link">View Job</a>
        </div>
    `;

  showNotificationElement(notificationElement);
}

// Generic function to show notification element
function showNotificationElement(notificationElement) {
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

// Đổi tên hàm showNotification cũ thành showNotification mới
// để đảm bảo tương thích với mã hiện tại
function showNotification(content) {
  if (typeof content === "string") {
    // Nếu là chuỗi, tạo phần tử thông báo từ chuỗi
    const notificationElement = document.createElement("div");
    notificationElement.className = "notification";
    notificationElement.innerHTML = `
      <div class="notification-content">
        ${content}
      </div>
    `;
    showNotificationElement(notificationElement);
  } else {
    // Nếu là phần tử HTML, sử dụng hàm mới
    showNotificationElement(content);
  }
}

// Start the connection when the document is ready
document.addEventListener("DOMContentLoaded", () => {
  startJobConnection();
});
