//// Jonas-OpenGl.cpp : This file contains the 'main' function. Program execution begins and ends there.
//#include <iostream>
//
//#define GLEW_STATIC
//
//#include <GL/glew.h>
//#include <GLFW/glfw3.h>
//
//const int WINDOW_WIDTH = 800;
//const int WINDOW_HEIGHT = 600;
//const char* APP_TITLE = "ST2024_Lara_Guelpen";
//GLFWwindow* gWindow;
//
////Vertex shader
//const GLchar* vertexShaderSrc = 
//"#version 330 core\n"
//"layout (location = 0) in vec3 pos;"
//"layout (location = 1) in vec3 color;"
//"out vec3 vert_color;"
//"void main ()"
//"{"
//"	vert_color = color;"
//"	gl_Position = vec4(pos.x, pos.y, pos.z, 1.0f);" //x, y, z, w
//"}";
//
////fragment shader
//const GLchar* fragmentShaderSrc = 
//"#version 330 core\n"
//"in vec3 vert_color;"
//"out vec4 frag_color;"
//"void main ()"
//"{"
//"	frag_color = vec4(vert_color, 1.0f);" //r, g, b, alpha
//"}";
//
//using namespace std;
//
//bool InitOpenGl ();
//void GlfwOnKey (GLFWwindow* window, int key, int scancode, int action, int mode);
//
//int main()
//{
//	if (!InitOpenGl ())
//	{
//		cerr << "OpenGl init failed!" << endl;
//		return -1;
//	}
//
//	//Init Triangle
//	//Define traingle points
//	GLfloat vertices[] =
//	{
//		//Each Row is one data packe!
//		//First 3 are position
//		//Second 3 are color
//
//		//First Triangle
//		-0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,//Top Left
//		0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, //Bottom right
//		-0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f,//Bottom left
//
//		//Second Trianlge
//		-0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,//Top Left
//		0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,//Top Right
//		0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, //Bottom right
//	};
//
//	//Vertex Buffer Object stores data in gpu memmory
//	GLuint vbo; //ID of vertex buffer object
//	
//	//Generate buffer with id vbo
//	glGenBuffers (1, &vbo); 
//
//	//Bind vbo to GL_ARRAY_BUFFER
//	glBindBuffer (GL_ARRAY_BUFFER, vbo);
//
//	//Copy data to GPU, Static data dont change, dynamic data change every frame
//	glBufferData (GL_ARRAY_BUFFER, sizeof (vertices), vertices, GL_STATIC_DRAW);
//
//	//Vertex Array Object
//	GLuint vao;
//	glGenVertexArrays (1, &vao);
//	glBindVertexArray (vao);
//
//	//Position
//	glVertexAttribPointer (0 /*Location for Shader*/, 3, GL_FLOAT, GL_FALSE /*normalized*/
//		, sizeof (GLfloat) * 6 /*Data package size, position(3) + color (3)*/, NULL);
//	glEnableVertexAttribArray (0);
//
//	//Color
//	glVertexAttribPointer (1 /*Location for Shader*/, 3, GL_FLOAT, GL_FALSE /*normalized*/
//		, sizeof (GLfloat) * 6 /*Data package size, position(3) + color (3)*/
//		, (GLvoid*) (sizeof (GLfloat) * 3)/*Memory address offet to first color entry*/);
//	glEnableVertexAttribArray (1);
//
//	//Create and compile shader
//	//Vertex Shader
//	GLuint vs = glCreateShader (GL_VERTEX_SHADER);
//	glShaderSource (vs, 1, &vertexShaderSrc, NULL);
//	glCompileShader (vs);
//
//	GLint result;
//	GLchar infoLog [512];
//	glGetShaderiv (vs, GL_COMPILE_STATUS, &result);
//
//	if (!result)
//	{
//		glGetShaderInfoLog (vs, sizeof (infoLog), NULL, infoLog);
//		cerr << "Error: Vertex shader failed to compile." << infoLog << endl;
//	}
//
//	//Fragment Shader
//	GLuint fs = glCreateShader (GL_FRAGMENT_SHADER);
//	glShaderSource (fs, 1, &fragmentShaderSrc, NULL);
//	glCompileShader (fs);
//
//	glGetShaderiv (fs, GL_COMPILE_STATUS, &result);
//
//	if (!result)
//	{
//		glGetShaderInfoLog (fs, sizeof (infoLog), NULL, infoLog);
//		cerr << "Error: Fragment shader failed to compile." << infoLog << endl;
//	}
//
//	//Create Shader Programm
//	GLuint shaderProgramm = glCreateProgram ();
//	glAttachShader (shaderProgramm, vs);
//	glAttachShader (shaderProgramm, fs);
//	glLinkProgram (shaderProgramm);
//
//	glGetProgramiv (shaderProgramm, GL_LINK_STATUS, &result);
//
//	if (!result)
//	{
//		glGetProgramInfoLog (shaderProgramm, sizeof (infoLog), NULL, infoLog);
//		cerr << "Error: Shader programm linker failure." << infoLog << endl;
//	}
//
//	//Free up shader memmory
//	glDeleteShader (vs);
//	glDeleteShader (fs);
//
//	//Main Lopp
//	while (!glfwWindowShouldClose(gWindow))
//	{
//		glfwPollEvents ();
//		glClear (GL_COLOR_BUFFER_BIT);
//
//		//Use Shader
//		glUseProgram (shaderProgramm);
//		glBindVertexArray (vao);
//		glDrawArrays (GL_TRIANGLES, 0, 6);
//		glBindVertexArray (0);
//
//		glfwSwapBuffers (gWindow);
//	}
//
//	//Free up memory
//	glDeleteProgram (shaderProgramm);
//	glDeleteVertexArrays (1, &vao);
//	glDeleteBuffers (1, &vbo);
//
//	glfwTerminate ();
//
//	return 0;
//}
//
//bool InitOpenGl ()
//{
//	if (glfwInit () != GLFW_TRUE)
//	{
//		cerr << "GLFW init failed!" << endl;
//		return false;
//	}
//
//	//Set GLFW Settings
//	glfwWindowHint (GLFW_CONTEXT_VERSION_MAJOR, 3);
//	glfwWindowHint (GLFW_CONTEXT_VERSION_MINOR, 3);
//	glfwWindowHint (GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
//	glfwWindowHint (GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
//
//	//Create Window
//	gWindow = glfwCreateWindow (WINDOW_WIDTH, WINDOW_HEIGHT, APP_TITLE, NULL, NULL);
//
//	if (gWindow == NULL)
//	{
//		cerr << "Window creation failed!" << endl;
//		glfwTerminate ();
//		return false;
//	}
//
//	glfwMakeContextCurrent (gWindow);
//
//	//Set Key callback method
//	glfwSetKeyCallback (gWindow, GlfwOnKey);
//	glewExperimental = GL_TRUE;
//
//	//Init GLEW
//	if (glewInit() != GLEW_OK)
//	{
//		cerr << "glew init failed!" << endl;
//		return false;
//	}
//
//	//Scene clear / background color
//	glClearColor (0.23f, 0.38f, 0.47f, 1.0f);
//
//	return true;
//}
//
//void GlfwOnKey (GLFWwindow* window, int key, int scancode, int action, int mode)
//{
//	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
//		glfwSetWindowShouldClose (window, GL_TRUE);
//}

#include <iostream>
#define GLEW_STATIC
#include "GL/glew.h"
#include "GLFW/glfw3.h"
#include "glm/glm.hpp"
#include "glm/gtc/matrix_transform.hpp"
#include "glm/gtc/type_ptr.hpp"
using namespace std;
const char* APP_TITLE = "ST2024 - PROCEDURAL GENERATION";
int WINDOW_WIDTH = 800;
int WINDOW_HEIGHT = 600;
GLFWwindow* gWindow;
const GLchar* vertexShaderSrc =
"#version 330 core\n"
"layout (location = 0) in vec3 pos;"
"layout (location=1) in vec3 color;"
"out vec3 vert_color;"
"uniform mat4 model;"
"uniform mat4 view;"
"uniform mat4 projection;"
"void main()"
"{"
" vert_color = color;"
" gl_Position = projection * view * model * vec4(pos, 1.0);"
"}";
const GLchar* fragmentShaderSrc =
"#version 330 core\n"
"in vec3 vert_color;"
"out vec4 frag_color;"
"void main()"
"{"
" frag_color = vec4(vert_color, 1.0f);" //r g b alpha
"}";
bool initOpenGL ();
void glfwOnKey (GLFWwindow* window, int key, int scancode, int action, int mode);
void glfwOnMouseMove (GLFWwindow* window, double xpos, double ypos);
void glfwOnFrameBufferSize (GLFWwindow* window, int width, int height);
void addMatrixTransformation (GLuint shaderProgram);

GLfloat* createTerrain (GLuint width, GLuint depth);
GLuint* createTriangles (GLuint width, GLuint depth);
GLfloat getHeight (float x, float z);

glm::vec3 getColor (GLfloat height, GLfloat maxHeight);

glm::vec3 gCameraPosition;
glm::vec3 gCameraLookAt;

float gCameraYaw; //y rotation
float gCameraPitch; //x rotation
const float cameraMovementSpeed = 0.5f;

double lastXPos, lastYPos;
const float mouseSensitivity = 0.1f;

int main ()
{
	if (!initOpenGL ())
	{
		cerr << "OpenGL initialization failed." << endl;
		return -1;
	}
	
	GLuint terrainWidth = 20;
	GLuint terrainDepth = 20;

	GLfloat* vertices = createTerrain (terrainWidth, terrainDepth);
	GLuint* indices = createTriangles (terrainWidth, terrainDepth);

	GLuint numVertices = (terrainWidth + 1) * (terrainDepth + 1);
	GLuint numIndices = terrainWidth * terrainDepth * 6;

	GLuint vbo;
	glGenBuffers (1, &vbo);
	glBindBuffer (GL_ARRAY_BUFFER, vbo);
	glBufferData (GL_ARRAY_BUFFER, numVertices * 6 * sizeof (GLuint), vertices, GL_STATIC_DRAW);
	GLuint vao;
	glGenVertexArrays (1, &vao);
	glBindVertexArray (vao);

	//position
	glVertexAttribPointer (0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof (GLfloat), NULL);
	glEnableVertexAttribArray (0);

	//color
	glVertexAttribPointer (1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof (GLfloat), (GLvoid*)(sizeof (GLfloat) * 3));
	glEnableVertexAttribArray (1);
	GLuint ebo;
	glGenBuffers (1, &ebo);
	glBindBuffer (GL_ELEMENT_ARRAY_BUFFER, ebo);
	glBufferData (GL_ELEMENT_ARRAY_BUFFER, numIndices * sizeof (GLuint), indices, GL_STATIC_DRAW);

	//Compile Shaders
	GLuint vs = glCreateShader (GL_VERTEX_SHADER);
	glShaderSource (vs, 1, &vertexShaderSrc, NULL);
	glCompileShader (vs);
	GLint result;
	GLchar infoLog[512];
	glGetShaderiv (vs, GL_COMPILE_STATUS, &result);
	if (!result)
	{
		glGetShaderInfoLog (vs, sizeof (infoLog), NULL, infoLog);
		cerr << "Error: Vertex shader failed to compile." << infoLog <<
			endl;
	}
	GLuint fs = glCreateShader (GL_FRAGMENT_SHADER);
	glShaderSource (fs, 1, &fragmentShaderSrc, NULL);
	glCompileShader (fs);
	glGetShaderiv (fs, GL_COMPILE_STATUS, &result);
	if (!result)
	{
		glGetShaderInfoLog (fs, sizeof (infoLog), NULL, infoLog);
		cerr << "Error: Fragment shader failed to compile." << infoLog <<
			endl;
	}
	GLuint shaderProgram = glCreateProgram ();
	glAttachShader (shaderProgram, vs);
	glAttachShader (shaderProgram, fs);
	glLinkProgram (shaderProgram);
	glGetProgramiv (shaderProgram, GL_LINK_STATUS, &result);
	if (!result)
	{
		glGetProgramInfoLog (shaderProgram, sizeof (infoLog), NULL,
			infoLog);
		cerr << "Error: shader programm linker failure" << infoLog <<
			endl;
	}
	glDeleteShader (vs);
	glDeleteShader (fs);

	gCameraPosition = glm::vec3 (30.0f, 30.0f, 30.0f);
	gCameraLookAt = glm::vec3 (10.0f, 0.0f, 10.0f);

	gCameraYaw = -120.f;
	gCameraPitch = 0.f;

	//Main Loop
	while (!glfwWindowShouldClose (gWindow))
	{
		glfwPollEvents ();
		glClear (GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		addMatrixTransformation (shaderProgram);
		glBindVertexArray (vao);
		//glDrawArrays(GL_TRIANGLES, 0, 6);
		glDrawElements (GL_TRIANGLES, numIndices * 6, GL_UNSIGNED_INT, 0);
		glBindVertexArray (0);
		glfwSwapBuffers (gWindow);
	}
	glDeleteProgram (shaderProgram);
	glDeleteVertexArrays (1, &vao);
	glDeleteBuffers (1, &vbo);
	glDeleteBuffers (1, &ebo);
	glfwTerminate ();
	return 0;
}
bool initOpenGL ()
{
	if (!glfwInit ())
	{
		cerr << "GLFW initialization failed." << endl;
		return false;
	}
	//Set GLFW parameters
	glfwWindowHint (GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint (GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint (GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint (GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	gWindow = glfwCreateWindow (WINDOW_WIDTH, WINDOW_HEIGHT, APP_TITLE,
		NULL, NULL);
	if (gWindow == NULL)
	{
			cerr << "Error window" << endl;
		glfwTerminate ();
		return false;
	}
	glfwMakeContextCurrent (gWindow);
	glfwSetKeyCallback (gWindow, glfwOnKey);
	glfwSetCursorPosCallback (gWindow, glfwOnMouseMove);
	glfwSetFramebufferSizeCallback (gWindow, glfwOnFrameBufferSize);
	glViewport (0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
	glewExperimental = GL_TRUE;
	if (glewInit () != GLEW_OK)
	{
		cerr << "Error glew" << endl;
		return false;
	}
	glClearColor (0.23f, 0.38f, 0.47f, 1.0f);
	glEnable (GL_DEPTH_TEST);
	return true;
}

void glfwOnMouseMove (GLFWwindow* window, double xpos, double ypos)
{
	float deltaX = xpos - lastXPos;
	float deltaY = lastYPos - ypos;

	deltaX *= mouseSensitivity;
	deltaY *= mouseSensitivity;

	gCameraYaw += deltaX;
	gCameraPitch += deltaY;

	if (gCameraPitch > 89.f) gCameraPitch = 89.f;
	else if (gCameraPitch < -89.f) gCameraPitch = -89.f;

	glm::vec3 forward;
	forward.x = cos (glm::radians (gCameraYaw)) * cos (glm::radians (gCameraPitch));
	forward.y = sin (glm::radians (gCameraPitch));
	forward.z = sin (glm::radians (gCameraYaw)) * cos (glm::radians (gCameraPitch));

	gCameraLookAt = gCameraPosition + glm::normalize (forward);

	lastXPos = xpos;
	lastYPos = ypos;
}

void glfwOnKey (GLFWwindow* window, int key, int scancode, int action, int
	mode)
{
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose (window, GL_TRUE);

	glm::vec3 forward = glm::normalize (gCameraLookAt - gCameraPosition);
	glm::vec3 right = glm::normalize (glm::cross (forward, glm::vec3(0.f, 1.f, 0.f)));

	//Camera movement
	if (key == GLFW_KEY_W && (action == GLFW_PRESS || action == GLFW_REPEAT))
	{
		gCameraPosition += forward * cameraMovementSpeed;
		gCameraLookAt += forward * cameraMovementSpeed;
	}
	if (key == GLFW_KEY_S && (action == GLFW_PRESS || action == GLFW_REPEAT))
	{
		gCameraPosition -= forward * cameraMovementSpeed;
		gCameraLookAt -= forward * cameraMovementSpeed;
	}
	if (key == GLFW_KEY_D && (action == GLFW_PRESS || action == GLFW_REPEAT))
	{
		gCameraPosition += right * cameraMovementSpeed;
		gCameraLookAt += right * cameraMovementSpeed;
	}
	if (key == GLFW_KEY_A && (action == GLFW_PRESS || action == GLFW_REPEAT))
	{
		gCameraPosition -= right * cameraMovementSpeed;
		gCameraLookAt -= right * cameraMovementSpeed;
	}
}
void glfwOnFrameBufferSize (GLFWwindow* window, int width, int height)
{
	WINDOW_WIDTH = width;
	WINDOW_HEIGHT = height;
	glViewport (0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
}
void addMatrixTransformation (GLuint shaderProgram)
{
	glm::mat4 model = glm::mat4 (1.0f);
	glm::mat4 view = glm::lookAt (
		gCameraPosition, // Camera position in world space
		gCameraLookAt, // Look at point in world space
		glm::vec3 (0.0f, 1.0f, 0.0f)); // Up vector
	glm::mat4 projection = glm::perspective (
		glm::radians (45.0f), // Field of View in degrees 
		(float)WINDOW_WIDTH / (float)WINDOW_HEIGHT, // Aspect Ratio
		0.1f, // Near clipping plane
		100.0f); // Far clipping plane
	glUseProgram (shaderProgram);
	glUniformMatrix4fv (
		glGetUniformLocation (shaderProgram, "model"), //Location of model matrix in shader
		1, // Number of matrices to send
		GL_FALSE, // Whether to transprose the matrix
		glm::value_ptr (model)); // Pointer to the matrix data 
	glUniformMatrix4fv (glGetUniformLocation (shaderProgram, "view"), 1,
		GL_FALSE, glm::value_ptr (view));
	glUniformMatrix4fv (glGetUniformLocation (shaderProgram, "projection"),
		1, GL_FALSE, glm::value_ptr (projection));
}

GLfloat* createTerrain (GLuint width, GLuint depth)
{
	GLfloat terrainHeight = 8.f;
	GLfloat* vertices = new GLfloat[(width + 1) * (depth + 1) * 6];

	for (int z = 0, i = 0; z <= depth; ++z)
	{
		for (int x = 0; x <= width; x++, i += 6)
		{
			vertices[i] = x;
			vertices[i + 1] = getHeight (x, z) * terrainHeight;
			vertices[i + 2] = z;

			glm::vec3 color = getColor (vertices[i + 1], terrainHeight);
			vertices[i + 3] = color.r; //R
			vertices[i + 4] = color.g; //G
			vertices[i + 5] = color.b; //B
		}
	}

	return vertices;
}

GLuint* createTriangles (GLuint width, GLuint depth)
{
	GLuint* indices = new GLuint[6 * width * depth];
	int currentVertice = 0;
	int numOfTriangles = 0;

	for (int z = 0; z < depth; z++)
	{
		for (int x = 0; x < width; x++)
		{
			indices[numOfTriangles + 0] = currentVertice + 0;
			indices[numOfTriangles + 1] = currentVertice + width + 1;
			indices[numOfTriangles + 2] = currentVertice + 1;
			indices[numOfTriangles + 3] = currentVertice + 1;
			indices[numOfTriangles + 4] = currentVertice + width + 1;
			indices[numOfTriangles + 5] = currentVertice + width + 2;

			numOfTriangles += 6;
			currentVertice++;
		}
		currentVertice++;
	}
	return indices;
}

GLfloat getHeight (float x, float z)
{
	return ((sin(x) + 1) / 2.0f) * (cos(z) / 2.0f);
}

glm::vec3 getColor (GLfloat height, GLfloat maxHeight)
{
	GLfloat normalizedHeight = height / maxHeight;

	if (normalizedHeight < 0.25f)
		return glm::vec3 (0.2f, 0.0f, 1.0f);
	else if (normalizedHeight < 0.5f)
		return glm::vec3 (0.3f, 0.8f, 0.7f);
	else if (normalizedHeight < 0.75f)
		return glm::vec3 (0.5f, 0.6f, 0.2f);
	else
		return glm::vec3 (0.2f, 0.7f, 0.f);
}