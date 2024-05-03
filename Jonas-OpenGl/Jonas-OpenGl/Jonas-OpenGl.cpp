// Jonas-OpenGl.cpp : This file contains the 'main' function. Program execution begins and ends there.
#include <iostream>

#define GLEW_STATIC

#include <GL/glew.h>
#include <GLFW/glfw3.h>

const int WINDOW_WIDTH = 800;
const int WINDOW_HEIGHT = 600;
const char* APP_TITLE = "ST2024_Lara_Guelpen";
GLFWwindow* gWindow;

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

	std::cout << "Hello World!\n";

	while (!glfwWindowShouldClose(gWindow))
	{
		glfwPollEvents ();
		glClear (GL_COLOR_BUFFER_BIT);
		glfwSwapBuffers (gWindow);
	}

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

	glfwWindowHint (GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint (GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint (GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint (GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);

	gWindow = glfwCreateWindow (WINDOW_WIDTH, WINDOW_HEIGHT, APP_TITLE, NULL, NULL);

	if (gWindow == NULL)
	{
		cerr << "Window creation failed!" << endl;
		glfwTerminate ();
		return false;
	}

	glfwMakeContextCurrent (gWindow);
	glfwSetKeyCallback (gWindow, GlfwOnKey);
	glewExperimental = GL_TRUE;

	if (glewInit() != GLEW_OK)
	{
		cerr << "glew init failed!" << endl;
		return false;
	}

	glClearColor (0.23f, 0.38f, 0.47f, 1.0f);

	return true;
}

void GlfwOnKey (GLFWwindow* window, int key, int scancode, int action, int mode)
{
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose (window, GL_TRUE);
}