package main;
import java.io.File;
import java.io.FilenameFilter;
import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import org.apache.tools.ant.Project;
import org.apache.tools.ant.taskdefs.Zip;
import org.apache.tools.ant.types.FileSet;

public class JsPackage {
	private static String workPath = System.getProperty("user.dir");
	private static List<String> absolutePathList = new LinkedList<>();
	private static List<String> simplePathList = new LinkedList<>();
	
	final static void showAllFiles(File dir, final String filter) throws Exception {
		FilenameFilter filenameFilter = new FilenameFilter() {
			@Override
			public boolean accept(File file, String fileName) {
				if(fileName.indexOf(filter) > 0) {
					return true;
				}
				return false;
			}
		};
		
		File[] fs = dir.listFiles(filenameFilter);
		for (int i = 0; i < fs.length; i++) {			
			if (fs[i].isDirectory()) {
				try {
					showAllFiles(fs[i], filter);
				} catch (Exception e) {
					e.printStackTrace();
				}
			} else {
				System.out.println("[" + (i+1) + "]" + "find js file: " + fs[i].getAbsolutePath());
				absolutePathList.add(fs[i].getAbsolutePath());
				simplePathList.add(fs[i].getName());
			}
		}
	}
	
	public static void compress(String srcPathName) {  
        File srcDir = new File(srcPathName);  
        File zipFile = new File(srcDir.getName() + ".zip");

        Project prj = new Project();  
        FileSet fileSet = new FileSet(); 
        fileSet.setProject(prj);  
        fileSet.setDir(srcDir);
        
        Zip zip = new Zip();  
        zip.setProject(prj);  
        zip.setDestFile(zipFile);  
        zip.addFileset(fileSet);  
        zip.execute();
        
        System.out.println("compress successful: " + srcPathName + srcDir.getName() + ".zip");
    }  
	
	public static void main(String[] args) {
		if (args.length > 0) {
			workPath = args[0];
			if (!workPath.endsWith("/") && !workPath.endsWith("\\")) {
				workPath += File.separator;
			}
		}

		// 路径
		File srcFile = new File(workPath);
		if (!srcFile.exists()) {
			System.err.println("workpath not exist: " + workPath);
			return;
		}

		// 是否为目录
		if (!srcFile.isDirectory()) {
			System.err.println("workpath not be directory: " + workPath);
			return;
		}

		System.out.println("workpath: " + workPath);
		
		// 遍历
		try {
			showAllFiles(srcFile, ".js");
		} catch (Exception e) {
			e.printStackTrace();
		}
		
		String output = "output" + File.separator;
		for(int i=0; i < absolutePathList.size();i++) {
			String filePath = absolutePathList.get(i);
			String fileName = simplePathList.get(i);
			String[] fileNames = fileName.split("\\.");
			fileName = fileNames[0] + ".min." + fileNames[1]; 
			
			String command = "java -jar compiler.jar --js " + filePath + " --js_output_file " + workPath + output + fileName;
			try {
				System.out.println("[" + (i+1) + "]" + command);
				Runtime.getRuntime().exec(command);
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		
		// 目录文件压缩
		System.out.println("compress output folder: " + workPath + output);
		compress(workPath + output);
		
		try {
			// 提示退出
			System.out.println("press any key to exit.");
			System.in.read();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
