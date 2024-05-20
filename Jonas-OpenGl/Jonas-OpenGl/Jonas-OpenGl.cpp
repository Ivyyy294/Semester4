// Jonas-OpenGl.cpp : This file contains the 'main' function. Program execution begins and ends there.
#include <iostream>

#define GLEW_STATIC

#include <GL/glew.h>
#include <GLFW/glfw3.h>

const int WINDOW_WIDTH = 800;
const int WINDOW_HEIGHT = 600;
const char* APP_TITLE = "ST2024_Lara_Guelpen";
GLFWwindow* gWindow;

//Vertex shader
const GLchar* vertexShaderSrc = 
"#version 330 core\n"
"layout (location = 0) in vec3 pos;"
"layout (location = 1) in vec3 color;"
"out vec3 vert_color;"
"void main ()"
"{"
"	vert_color = color;"
"	gl_Position = vec4(pos.x, pos.y, pos.z, 1.0f);" //x, y, z, w
"}";

//fragment shader
const GLchar* fragmentShaderSrc = 
"#version 330 core\n"
"in vec3 vert_color;"
"out vec4 frag_color;"
"void main ()"
"{"
"	frag_color = vec4(vert_color, 1.0f);" //r, g, b, alpha
"}";

using namespace std;

bool InitOpenGl ();
void GlfwOnKey (GLFWwindow* window, int key, int scancode, int action, int mode);

int main()
{
	if (!InitOpenGl ())
	{
		cerr << "OpenGl init failed!" << endl;
		return -1;
	}

	//Init Triangle
	//Define traingle points
	GLfloat vertices[] =
	{
		//Each Row is one data packe!
		//First 3 are position
		//Second 3 are color

		//First Triangle
		-0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,//Top Left
		0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, //Bottom right
		-0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f,//Bottom left

		//Second Trianlge
		-0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,//Top Left
		0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,//Top Right
		0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, //Bottom right
	};

	//Vertex Buffer Object stores data in gpu memmory
	GLuint vbo; //ID of vertex buffer object
	
	//Generate buffer with id vbo
	glGenBuffers (1, &vbo); 

	//Bind vbo to GL_ARRAY_BUFFER
	glBindBuffer (GL_ARRAY_BUFFER, vbo);

	//Copy data to GPU, Static data dont change, dynamic data change every frame
	glBufferData (GL_ARRAY_BUFFER, sizeof (vertices), vertices, GL_STATIC_DRAW);

	//Vertex Array Object
	GLuint vao;
	glGenVertexArrays (1, &vao);
	glBindVertexArray (vao);

	//Position
	glVertexAttribPointer (0 /*Location for Shader*/, 3, GL_FLOAT, GL_FALSE /*normalized*/
		, sizeof (GLfloat) * 6 /*Data package size, position(3) + color (3)*/, NULL);
	glEnableVertexAttribArray (0);

	//Color
	glVertexAttribPointer (1 /*Location for Shader*/, 3, GL_FLOAT, GL_FALSE /*normalized*/
		, sizeof (GLfloat) * 6 /*Data package size, position(3) + color (3)*/
		, (GLvoid*) (sizeof (GLfloat) * 3)/*Memory address offet to first color entry*/);
	glEnableVertexAttribArray (1);

	//Create and compile shader
	//Vertex Shader
	GLuint vs = glCreateShader (GL_VERTEX_SHADER);
	glShaderSource (vs, 1, &vertexShaderSrc, NULL);
	glCompileShader (vs);

	GLint result;
	GLchar infoLog [512];
	glGetShaderiv (vs, GL_COMPILE_STATUS, &result);

	if (!result)
	{
		glGetShaderInfoLog (vs, sizeof (infoLog), NULL, infoLog);
		cerr << "Error: Vertex shader failed to compile." << infoLog << endl;
	}

	//Fragment Shader
	GLuint fs = glCreateShader (GL_FRAGMENT_SHADER);
	glShaderSource (fs, 1, &fragmentShaderSrc, NULL);
	glCompileShader (fs);

	glGetShaderiv (fs, GL_COMPILE_STATUS, &result);

	if (!result)
	{
		glGetShaderInfoLog (fs, sizeof (infoLog), NULL, infoLog);
		cerr << "Error: Fragment shader failed to compile." << infoLog << endl;
	}

	//Create Shader Programm
	GLuint shaderProgramm = glCreateProgram ();
	glAttachShader (shaderProgramm, vs);
	glAttachShader (shaderProgramm, fs);
	glLinkProgram (shaderProgramm);

	glGetProgramiv (shaderProgramm, GL_LINK_STATUS, &result);

	if (!result)
	{
		glGetProgramInfoLog (shaderProgramm, sizeof (infoLog), NULL, infoLog);
		cerr << "Error: Shader programm linker failure." << infoLog << endl;
	}

	//Free up shader memmory
	glDeleteShader (vs);
	glDeleteShader (fs);

	//Main Lopp
	while (!glfwWindowShouldClose(gWindow))
	{
		glfwPollEvents ();
		glClear (GL_COLOR_BUFFER_BIT);

		//Use Shader
		glUseProgram (shaderProgramm);
		glBindVertexArray (vao);
		glDrawArrays (GL_TRIANGLES, 0, 6);
		glBindVertexArray (0);

		glfwSwapBuffers (gWindow);
	}

	//Free up memory
	glDeleteProgram (shaderProgramm);
	glDeleteVertexArrays (1, &vao);
	glDeleteBuffers (1, &vbo);

	glfwTerminate ();

	return 0;
}

bool InitOpenGl ()
{
	if (glfwInit () != GLFW_TRUE)
	{
		cerr << "GLFW init failed!" << endl;
		return false;
	}

	//Set GLFW Settings
	glfwWindowHint (GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint (GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint (GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint (GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);

	//Create Window
	gWindow = glfwCreateWindow (WINDOW_WIDTH, WINDOW_HEIGHT, APP_TITLE, NULL, NULL);

	if (gWindow == NULL)
	{
		cerr << "Window creation failed!" << endl;
		glfwTerminate ();
		return false;
	}

	glfwMakeContextCurrent (gWindow);

	//Set Key callback method
	glfwSetKeyCallback (gWindow, GlfwOnKey);
	glewExperimental = GL_TRUE;

	//Init GLEW
	if (glewInit() != GLEW_OK)
	{
		cerr << "glew init failed!" << endl;
		return false;
	}

	//Scene clear / background color
	glClearColor (0.23f, 0.38f, 0.47f, 1.0f);

	return true;
}

void GlfwOnKey (GLFWwindow* window, int key, int scancode, int action, int mode)
{
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose (window, GL_TRUE);
}