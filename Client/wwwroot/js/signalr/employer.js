"use strict";

// Khởi tạo kết nối SignalR cho employer
document.addEventListener("DOMContentLoaded", function () {
  // Kiểm tra xem có phải là employer không
  const employerId =
    document.getElementById("current-employer-id")?.value ||
    localStorage.getItem("employerId");

  if (employerId) {
    console.log(
      `Initializing employer connection for employer ID: ${employerId}`
    );

    // Lưu employerId vào localStorage để sử dụng sau này
    localStorage.setItem("employerId", employerId);

    // Tạo kết nối đến JobHub
    const employerConnection = new signalR.HubConnectionBuilder()
      .withUrl("/jobHub")
      .withAutomaticReconnect()
      .build();

    // Xử lý sự kiện nhận thông báo ứng tuyển mới
    employerConnection.on("NewApplication", (jobId, applicantName) => {
      console.log(
        `Received new application notification: ${applicantName} applied for job ${jobId}`
      );

      // Hiển thị thông báo popup
      const notificationElement = document.createElement("div");
      notificationElement.className = "notification employer-notification";
      notificationElement.innerHTML = `
                <div class="notification-content">
                    <strong>Đơn ứng tuyển mới:</strong> ${applicantName} đã ứng tuyển vào công việc của bạn
                    <a href="/Employer/Applications" class="notification-link">Xem đơn ứng tuyển</a>
                </div>
            `;

      // Hiển thị thông báo
      const notificationContainer = document.getElementById(
        "notification-container"
      );
      if (notificationContainer) {
        notificationContainer.appendChild(notificationElement);

        // Tự động ẩn sau 5 giây
        setTimeout(() => {
          notificationElement.classList.add("fade-out");
          setTimeout(() => {
            notificationContainer.removeChild(notificationElement);
          }, 500);
        }, 5000);
      }

      // Thêm vào danh sách thông báo
      if (window.addNotification) {
        window.addNotification(
          "Đơn ứng tuyển mới",
          `${applicantName} đã ứng tuyển vào công việc của bạn`,
          "application",
          `/Employer/Applications`
        );
      }
    });

    // Xử lý sự kiện debug
    employerConnection.on("DebugNotification", (message) => {
      console.log(`Debug notification: ${message}`);
    });

    // Khởi động kết nối
    employerConnection
      .start()
      .then(() => {
        console.log("Employer connection established");

        // Đăng ký tham gia nhóm employer
        return employerConnection.invoke("JoinAsEmployer", employerId);
      })
      .then(() => {
        console.log(`Successfully joined as Employer ${employerId}`);
      })
      .catch((err) => {
        console.error("Error establishing employer connection: " + err);

        // Thử kết nối lại sau 5 giây
        setTimeout(() => {
          console.log("Attempting to reconnect...");
          employerConnection
            .start()
            .then(() => {
              console.log("Reconnected successfully");
              return employerConnection.invoke("JoinAsEmployer", employerId);
            })
            .then(() => {
              console.log(
                `Successfully joined as Employer ${employerId} after reconnection`
              );
            })
            .catch((err) => {
              console.error("Failed to reconnect: " + err);
            });
        }, 5000);
      });

    // Xử lý sự kiện đóng kết nối
    employerConnection.onclose((error) => {
      console.log("Employer connection closed");
      if (error) {
        console.error("Connection closed due to error: " + error);
      }

      // Thử kết nối lại sau 5 giây
      setTimeout(() => {
        employerConnection
          .start()
          .then(() => {
            console.log("Reconnected after connection closed");
            return employerConnection.invoke("JoinAsEmployer", employerId);
          })
          .catch((err) => {
            console.error(
              "Failed to reconnect after connection closed: " + err
            );
          });
      }, 5000);
    });
  }
});
