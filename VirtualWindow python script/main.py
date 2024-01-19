import cv2
import socket

face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
unity_ip = "127.0.0.1"
unity_port = 12345

unity_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
unity_socket.connect((unity_ip, unity_port))
cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(gray, scaleFactor=1.3, minNeighbors=5)

    for (x, y, w, h) in faces:
        cv2.rectangle(frame, (x, y), (x+w, y+h), (255, 0, 0), 2)
        print(f"Face Position - X: {x}, Y: {y}")

        x_to_unity = x / 440
        y_to_unity = y / 330

        message = f"{x_to_unity},{y_to_unity}"
        print(f"Sending message: {message}")
        unity_socket.send(message.encode())

    cv2.imshow('FaceCam', frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
unity_socket.close()
